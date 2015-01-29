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

            var logEntriesService =
                   new NodeLogEntriesService(
                       new PersistentNodeState()
                       {
                           NodeId = 1,
                           Term = 2,
                           EntryIndex = 0,
                           LogEntries = new List<LogEntry>()
                       });

            var registry = new DomainRegistry()
               .UseDomainMessageSender(bus)
               .UseNodeMessageSender(bus)
               .UseNodeLogEntriesService(logEntriesService)
               .UseToReceiveMessages(bus)
               .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 });

           

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