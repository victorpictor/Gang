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

            DomainRegistry
                .RegisterService(
                    new NodeLogEntriesService(
                        new PersistentNodeState()
                        {
                            NodeId = 1,
                            Term = 2,
                            EntryIndex = 0,
                            LogEntries = new List<LogEntry>()
                        }));
            
            node = new Node(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, HeartBeatPeriod = 150, Majority = 3 },
                            state,
                            bus1,
                            bus2
                );
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
            var ns = DomainRegistry.NodLogEntriesService().NodeState();

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