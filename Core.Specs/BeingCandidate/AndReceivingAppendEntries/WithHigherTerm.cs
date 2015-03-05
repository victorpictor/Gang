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

            var logEntryStore = new LogEntryStore();
            logEntryStore.Append(new LogEntry()
            {
                NodeId = 1,
                Term = 2,
                Index = 0,
                MachineCommands = new List<object>()
            });

            var registry = new DomainRegistry()
                .UseNodeSettings(new NodeSettings() {NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3})
                .UseContolMessageQueue()
                .UseContolMessageSender(bus)
                .UseToReceiveMessages(bus)
                .UseNodeMessageSender(bus)
                .UseLogEntryStore(logEntryStore);

            node = new Node(finitState, registry);
        }

        public override void When()
        {
            node.Start();

            bus.Send(new AppendEntries(1, 4, 4, 4, 3, null));
            Thread.Sleep(900);

            //node.Stop();
        }

        [Test]
        public void It_should_update_term()
        {
            Assert.AreEqual(4, node.GetRegistry().LogEntriesService().NodeState().Term);
        }

        [Test]
        public void It_should_become_follower()
        {
            Assert.AreEqual(typeof(Follower), node.LastFinitState().GetType());
        }
    }
}