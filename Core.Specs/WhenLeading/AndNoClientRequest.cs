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
              .UseToReceiveMessages(bus2)
              .UseLogEntryStore(logEntryStore);


            node = new Node(state,registry);
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