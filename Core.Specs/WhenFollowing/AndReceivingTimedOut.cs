using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
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

            var logEntryStore = new LogEntryStore();
            logEntryStore.Append(new LogEntry()
            {
                NodeId = 1,
                Term = 1,
                Index = 0,
                MachineCommands = new List<object>()
            });

           var registry = new DomainRegistry()
               .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", Majority = 3, FollowerTimeout = 200})
               .UseContolMessageQueue()
               .UseNodeMessageSender(bus)
               .UseToReceiveMessages(bus)
               .UseLogEntryStore(logEntryStore);

            node = new Node(state, registry);
        }

        public override void When()
        {
            node.Start();
            Thread.Sleep(1000);
            node.Stop();
            Thread.Sleep(300);
        }

        [Test]
        public void It_should_become_Candidate()
        {
            Assert.AreEqual(typeof(Candidate), node.LastFinitState().GetType());
        }

        [Test]
        public void It_should_increment_term()
        {
            var term = node.GetRegistry().LogEntriesService().NodeState().Term;
            Assert.GreaterOrEqual(term, 2);
        }

    }
}