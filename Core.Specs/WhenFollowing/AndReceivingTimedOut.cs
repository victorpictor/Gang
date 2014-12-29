using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.States;
using Core.States.TheCandidate;
using Core.States.TheFollower;
using NUnit.Framework;

namespace Core.Specs.WhenFollowing
{
    [TestFixture]
    public class AndReceivingTimedOut : Specification
    {
        private Follower state;
        private Node node;

        public override void Given()
        {

            state = new Follower();

            var bus = new InMemoryBus();

            DomainRegistry
                .RegisterService(
                    new NodeLogEntriesService(
                        new PersistentNodeState()
                            {
                                NodeId = 1,
                                Term = 1,
                                EntryIndex = 0,
                                LogEntries = new List<LogEntry>()
                            }));

            node = new Node(new NodeSettings() {NodeId = 1, NodeName = "N1", ElectionTimeout = 500, Majority = 3},
                            state, 
                            bus,
                            bus
                );
        }

        public override void When()
        {
            node.Start();
            Thread.Sleep(1500);
            node.Stop();
        }

        [Test]
        public void It_should_become_Candidate()
        {
            Assert.AreEqual(typeof(Candidate),node.LastFinitState().GetType());
        }

        [Test]
        public void It_should_increment_term()
        {
            var term = DomainRegistry.NodLogEntriesService().NodeState().Term;
            Assert.AreEqual(2, term);
        }

    }
}