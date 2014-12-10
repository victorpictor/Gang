using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States;
using Core.States.TheFollower;
using NUnit.Framework;

namespace Core.Specs.WhenFollowing.AndReceivingAppendEntries
{
    [TestFixture]
    public class WithHigherTerm : Specification
    {
        private Follower state;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {

            state = new Follower();

            bus = new InMemoryBus();

            node = new Node(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 },
                            new PersistentNodeState()
                            {
                                NodeId = 1,
                                Term = 2,
                                EntryIndex = 0,
                                LogEntries = new List<LogEntry>()
                            },
                            state,
                            bus,
                            bus
                );
        }

        public override void When()
        {
            node.Start();

            bus.Send(new AppendEntries() { Term = 4 });
            Thread.Sleep(900);
            node.Stop();
        }

        [Test]
        public void It_should_not_ignore_append_entry_command()
        {
            var controllMessage = bus.messages.Dequeue();
            Assert.AreEqual(0, bus.MessageCount());
        }

        [Test]
        public void It_should_update_term()
        {
            Assert.AreEqual(node.GetState().Term, 4);
        }

        [Test]
        public void It_should_stay_in_follower_state()
        {
            Assert.AreEqual(node.LastFinitState().GetType(), typeof(Follower));
        }
    }
}