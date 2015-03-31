using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.Receivers;
using Core.Senders;
using Core.States.TheLead;
using Core.Transport;
using Moq;
using NUnit.Framework;

namespace Core.Specs.WhenLeading.HeartBeat
{
    public class MyLeader : Leader
    {
        public MyLeader(): base()
        {
            var receiver = new Mock<IReceiveMessages<IMessage>>();
            receiver.SetReturnsDefault(new ClientCommand(){Id = Guid.NewGuid(), Command = new object()});

            LeaderBus.InitLeaderBus(new Mock<IReceiveMessages<IClientCommand>>().Object, new Mock<ISend<ClientReply>>().Object, receiver.Object);
        }

        public void SetLastRequestTime()
        {
            base.requestState = new RequestState(){};
        }
    }

    [TestFixture]
    public class AndClientSubmitsRequests : Specification
    {
        private MyLeader state;
        private Node node;

        private InMemoryBus bus1;
        private InMemoryBus bus2;

        public override void Given()
        {
            state = new MyLeader();
            
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
               .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, HeartBeatPeriod = 250, Majority = 3 })
               .UseContolMessageQueue()
               .UseNodeMessageSender(bus1)
               .UseLogEntryStore(logEntryStore)
               .UseToReceiveMessages(bus2);

            node = new Node(state,registry);
        }

        public override void When()
        {
            node.Start();

            for (int i = 0; i < 5; i++)
            {
                state.SetLastRequestTime();
                Thread.Sleep(50);
            }
            
            node.Stop();
            Thread.Sleep(1000);
        }

        [Test]
        public void It_should_not_send_heart_beat_to_nodes()
        {
           Assert.LessOrEqual(0, bus1.MessageCount());
        }
    }
}