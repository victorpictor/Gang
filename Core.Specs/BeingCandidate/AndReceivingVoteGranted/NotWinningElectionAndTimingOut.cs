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
              .UseNodeMessageSender(bus)
              .UseNodeLogEntriesService(logEntriesService)
              .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 300, Majority = 3 });

            node = new Node(state,registry,bus);
        }


        public override void When()
        {
            node.Start();

            bus.Send(new VoteGranted() {Term = 1, VoterId = 2});

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
            Assert.AreEqual(2, node.GetRegistry().UseLogEntriesService().NodeState().Term);
        }
    }
}
