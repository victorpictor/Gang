using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States;
using Core.States.TheCandidate;
using NUnit.Framework;

namespace Core.Specs.BeingCandidate.AndReceivingVoteGranted
{
    [TestFixture]
    public class NotWinningElectionAndTimingOut : Specification
    {

        private Candidate state;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {
            state = new Candidate();

            bus = new InMemoryBus();

            var logEntryStore = new LogEntryStore();
            logEntryStore.Append(new LogEntry()
            {
                NodeId = 1,
                Term = 1,
                Index = 0,
                MachineCommands = new List<object>()
            });

            var registry = new DomainRegistry()
                .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 300, Majority = 3 })
                .UseDomainMessageSender(bus)
                .UseToReceiveMessages(bus)
                .UseLogEntryStore(logEntryStore);

            node = new Node(state,registry);
        }


        public override void When()
        {
            node.Start();

            bus.Send(new VoteGranted(1, 2, 1));

            Thread.Sleep(900);
        }

        [Test]
        public void It_should_stay_Candidate()
        {
            Assert.AreEqual(typeof (Candidate), node.LastFinitState().GetType());
        }

        [Test]
        public void It_should_increment_state()
        {
            Assert.AreEqual(2, node.GetRegistry().LogEntriesService().NodeState().Term);
        }
    }
}
