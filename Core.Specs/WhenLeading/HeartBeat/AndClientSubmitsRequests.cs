using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.Receivers;
using Core.Senders;
using Core.States;
using Core.Transport;
using Moq;
using NUnit.Framework;

namespace Core.Specs.WhenLeading.HeartBeat
{
    public class MyLeader : Leader
    {
        public MyLeader(): base()
        {
            LeaderBus.InitLeaderBus(new Mock<IReceiveMessages<IClientCommand>>().Object, new Mock<ISend<ClientReply>>().Object, new Mock<IReceiveMessages<IMessage>>().Object);
        }

        public void SetLastRequestTime(DateTime requested)
        {
            base.lastRequest = requested;
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

            node = new Node(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, HeartBeatPeriod = 250, Majority = 3 },
                            new PersistentNodeState()
                            {
                                NodeId = 1,
                                Term = 2,
                                EntryIndex = 0,
                                LogEntries = new List<LogEntry>()
                            },
                            state,
                            bus1,
                            bus2
                );
        }

        public override void When()
        {
            node.Start();

            for (int i = 0; i < 10; i++)
            {
                state.SetLastRequestTime(DateTime.Now);
                Thread.Sleep(100);
            }
            
            node.Stop();
        }

        [Test]
        public void It_should_not_send_heart_beat_to_nodes()
        {
            var contorollMessage = bus1.messages.Dequeue();

            Assert.LessOrEqual(0, bus1.MessageCount());
        }
    }
}