using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.States;
using Core.States.TheCandidate;
using NUnit.Framework;

namespace Core.Specs.BeingCandidate
{
    [TestFixture]
    public class AndRequestingVotes : Specification
    {
        private Candidate state;
        private Node node;

        private InMemoryBus bus;
        private InMemoryBus bus2;

        public override void Given()
        {
            state = new Candidate();

            bus = new InMemoryBus();
            bus2 = new InMemoryBus();

            var logEntryStore = new LogEntryStore();
            logEntryStore.Append(new LogEntry()
            {
                NodeId = 1,
                Term = 1,
                Index = 0,
                MachineCommands = new List<object>()
            });

            var registry = new DomainRegistry()
                .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 1000, Majority = 3 })
                .UseNodeMessageSender(bus2)
                .UseLogEntryStore(logEntryStore)
                .UseToReceiveMessages(bus);

            node = new Node(state,registry);
        }


        public override void When()
        {
            node.Start();
            Thread.Sleep(3000);
        }

        [Test]
        public void It_should_stay_Candidate()
        {
            Assert.AreEqual(typeof(Candidate), node.LastFinitState().GetType());
        }

        [Test]
        public void It_should_publish_the_request_vote_message()
        {
            Assert.AreEqual(1, bus2.messages.Count);
        }
    }
}