using System.Collections.Generic;
using Core.Clustering;
using Core.Log;
using Core.States;
using NUnit.Framework;

namespace Core.Specs.WhenFollowing
{
    [TestFixture]
    public class AndTimingOut : Specification
    {
        private Follower state;
        private Node node;

        public override void Given()
        {

            state = new Follower();

            node = new Node(new NodeSettings() {NodeId = 1, NodeName = "N1", ElectionTimeout = 5000, Majority = 3},
                            new PersistentNodeState()
                                {
                                    NodeId = 1,
                                    Term = 1,
                                    EntryIndex = 0,
                                    LogEntries = new List<LogEntry>()
                                },
                            state
                );
        }

        public override void When()
        {
            node.Start();
        }

        [Test]
        public void It_should_become_Candidate()
        {
            Assert.AreEqual(node.LastFinitState().GetType(), typeof(Candidate));
        }

    }
}