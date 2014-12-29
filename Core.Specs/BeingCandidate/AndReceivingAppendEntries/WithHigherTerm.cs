using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States;
using Core.States.TheCandidate;
using Core.States.TheFollower;
using NUnit.Framework;

namespace Core.Specs.BeingCandidate.AndReceivingAppendEntries
{
    [TestFixture]
    public class WithHigherTerm : Specification
    {
        private Candidate finitState;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {

            finitState = new Candidate();

            bus = new InMemoryBus();

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

            node = new Node(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 },
                            finitState,
                            bus,
                            bus
                );
        }

        public override void When()
        {
            node.Start();

            bus.Send(new AppendEntries() { Term = 4 });

            node.Stop();

            Thread.Sleep(900);
        }

        [Test]
        public void It_should_update_term()
        {
            Assert.AreEqual(4, DomainRegistry.NodLogEntriesService().NodeState().Term);
        }

        [Test]
        public void It_should_become_follower()
        {
            Assert.AreEqual(typeof(Follower), node.LastFinitState().GetType());
        }
    }
}