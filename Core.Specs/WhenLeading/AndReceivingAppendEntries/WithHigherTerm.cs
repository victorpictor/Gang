using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.Specs.WhenLeading.HeartBeat;
using Core.States.TheFollower;
using NUnit.Framework;

namespace Core.Specs.WhenLeading.AndReceivingAppendEntries
{
    [TestFixture]
    public class WithHigherTerm : Specification
    {
        private MyLeader state;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {

            state = new MyLeader();
            
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
                .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 })
               .UseDomainMessageSender(bus)
               .UseNodeMessageSender(bus)
               .UseLogEntryStore(logEntryStore)
               .UseToReceiveMessages(bus);

            node = new Node(state,registry);
        }

        public override void When()
        {
            node.Start();

            bus.Send(new AppendEntries(1, 4, 4, 2, 1, null));

            Thread.Sleep(900);
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