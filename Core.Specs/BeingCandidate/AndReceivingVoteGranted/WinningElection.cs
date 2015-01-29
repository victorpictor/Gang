using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States;
using Core.States.TheCandidate;
using Core.States.TheLead;
using NUnit.Framework;

namespace Core.Specs.BeingCandidate.AndReceivingVoteGranted
{
    [TestFixture]
    public class WinningElection : Specification
    {
        private Candidate state;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {
            state = new Candidate();

            bus = new InMemoryBus();

            var logEntriesService =
                    new NodeLogEntriesService(
                        new PersistentNodeState()
                        {
                            NodeId = 1,
                            Term = 1,
                            EntryIndex = 0,
                            LogEntries = new List<LogEntry>()
                        });

            var registry = new DomainRegistry()
             .UseDomainMessageSender(bus)
             .UseToReceiveMessages(bus)
             .UseNodeLogEntriesService(logEntriesService)
             .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 1000, Majority = 3 });

            node = new Node(state, registry);
        }


        public override void When()
        {
            node.Start();

            bus.Send(new VoteGranted(2, 55, 1));
            bus.Send(new VoteGranted(3, 55, 1));
            bus.Send(new VoteGranted(2, 55, 1));
            bus.Send(new VoteGranted(4, 55, 1));

            node.Stop();

            Thread.Sleep(900);
        }

        [Test]
        public void It_should_become_Candidate()
        {
            Assert.AreEqual(typeof(Leader), node.LastFinitState().GetType());
        }
    }
}