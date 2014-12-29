using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.States.TheLead;
using NUnit.Framework;

namespace Core.Specs.WhenLeading
{
    [TestFixture]
    public class AndNoClientRequest : Specification
    {
        private Leader state;
        private Node node;

        private InMemoryBus bus1;
        private InMemoryBus bus2;

        public override void Given()
        {
            state = new Leader();

            bus1 = new InMemoryBus();
            bus2 = new InMemoryBus();

            DomainRegistry
               .RegisterServiceFactory(
                   new ServiceFactory(
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

            Thread.Sleep(900);
        }

        [Test]
        public void It_should_send_heart_beat_to_nodes_every_beatperiod()
        {
            Assert.GreaterOrEqual(bus1.MessageCount(),6);
        }
    }
}