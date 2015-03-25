using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.Receivers;
using Core.Senders;
using Core.States;
using Core.States.TheLead;
using Core.Transport;
using NUnit.Framework;

namespace Core.Specs.WhenLeading.AndReceivingClientCommand
{
    [TestFixture]
    public class AndFollowersAck:Specification
    {
        private class ClientReplyBus : ISend<ClientReply>
        {
            public Queue<ClientReply> Replies = new Queue<ClientReply>();

            public void Send(ClientReply r)
            {
                Replies.Enqueue(r);
            }

            public void Reply(IReply m)
            {
                Replies.Enqueue(new ClientReply());
            }
        }
        private class ClientCommandReceiver : IReceiveMessages<IClientCommand>
        {
            public IClientCommand Receive()
            {
                return new ClientCommand() { Id = Guid.NewGuid(), Command = new object() };
            }
        }
        private class FollowerMessageReceiver : IReceiveMessages<IMessage>
        {
            private int Id = 2;
            
            public IMessage Receive()
            {
                Thread.Sleep(30);
                return new EntriesAppended() {NodeId = Id++,Term = 2, LogIndex = 1 };
            }
        }
        
        private Leader state;
       
        private Node node;

        private InMemoryBus bus1;
        private InMemoryBus bus2;
        private ClientReplyBus bus3;

        public override void Given()
        {
            bus3 = new ClientReplyBus();
            LeaderBus.InitLeaderBus(new ClientCommandReceiver(), bus3 , new FollowerMessageReceiver());

            state = new Leader();

            bus1 = new InMemoryBus();
            bus2 = new InMemoryBus();

            var logEntryStore = new LogEntryStore();
            logEntryStore.Append(new LogEntry()
            {
                NodeId = 1,
                Term = 2,
                Index = 0,
                MachineCommands = new List<object>()
            });

            var registry = new DomainRegistry()
              .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, HeartBeatPeriod = 150, Majority = 3 })
              .UseContolMessageQueue()
              .UseNodeMessageSender(bus1)
              .UseLogEntryStore(logEntryStore)
              .UseToReceiveMessages(bus2);

            node = new Node(state,registry);
        }

        public override void When()
        {
            node.Start();
            Thread.Sleep(1500);
            node.Stop();
        }
        

        [Test]
        public void It_should_send_message_to_followers()
        {
           Assert.IsTrue(bus1.MessageCount() > 1);
        }

        [Test]
        public void It_should_update_node_state()
        {
            var ns = node.GetRegistry().LogEntriesService().NodeState();

            Assert.AreEqual(2, ns.Term);
            Assert.AreEqual(1, ns.EntryIndex);
        }
        
        [Test]
        public void It_should_reply_to_client_once_ack_by_majority()
        {
            Assert.IsTrue(bus3.Replies.Count == 1);
        }
    }
}