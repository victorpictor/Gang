using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States;
using NUnit.Framework;

namespace Core.Specs.WhenFollowing.AndReceivingRequestedVote
{
    [TestFixture]
    public class WithLowerTerm : Specification
    {
        private MyFollower state;
        private Node node;

        public override void Given()
        {

            state = new MyFollower();

            bus = new InMemoryBus();

            var logEntryStore = new LogEntryStore();
            logEntryStore.Append(new LogEntry()
            {
                NodeId = 1,
                Term = 3,
                Index = 0,
                MachineCommands = new List<object>()
            });

            var registry = new DomainRegistry()
                .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 })
                .UseDomainMessageSender(bus)
                .UseNodeMessageSender(bus)
                .UseToReceiveMessages(bus)
                .UseLogEntryStore(logEntryStore);

            node = new Node(state,registry);
        }

        private InMemoryBus bus;

        public override void When()
        {
            node.Start();

            bus.Send(new RequestedVote(2,4,2,3));
            Thread.Sleep(1500);

            node.Stop();
        }

        [Test]
        public void It_should_stay_in_follower_state()
        {
            Assert.AreEqual(typeof(MyFollower), node.LastFinitState().GetType());
        }

        [Test]
        public void It_should_ignore_request_vote_entry_command()
        {
            var controlMessage = bus.Receive();
            Assert.AreEqual(0, bus.MessageCount());
        }
    }
}