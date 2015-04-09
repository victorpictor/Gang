using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.Specs.WhenLeading.HeartBeat;
using NUnit.Framework;

namespace Core.Specs.WhenLeading.AndReceivingRequestedVote
{
    [TestFixture]
    public class WithLowerTerm : Specification
    {
        private MyLeader state;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {
            state = new MyLeader();

            bus = new InMemoryBus();

            var logEntryStore = new LogEntryStore();
            logEntryStore.Append(new LogEntry()
            {
                NodeId = 1,
                Term = 2,
                Index = 5,
                MachineCommands = new List<object>()
            });

            var registry = new DomainRegistry()
                .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 })
               .UseContolMessageQueue()
               .UseNodeMessageSender(bus)
               .UseLogEntryStore(logEntryStore)
               .UseToReceiveMessages(bus);

            node = new Node(state, registry);
        }

        public override void When()
        {
            node.Start();

            bus.Send(new RequestedVote(2, 1, 1, 1));

            Thread.Sleep(900);
        }

        [Test]
        public void It_should_stay_Leader()
        {
            Assert.AreEqual(typeof(MyLeader), node.LastFinitState().GetType());
        }
    }
}