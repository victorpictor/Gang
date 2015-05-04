using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States;
using Core.States.TheLead;
using NUnit.Framework;

namespace Core.Specs.WhenLeading.AndReceivingAppendEntries
{
    [TestFixture]
    public class WithLowerTerm : Specification
    {
        private Leader state;
        private Node node;

        private InMemoryBus bus;
        
        public override void Given()
        {

            state = new Leader();

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
               .UseContolMessageQueue()
               .UseToReceiveClientCommands(bus)
               .UseNodeMessageSender(bus)
               .UseLogEntryStore(logEntryStore)
               .UseToReceiveMessages(bus);

            node = new Node(state, registry);
        }

        public override void When()
        {
            node.Start();

            bus.Send(new AppendEntries(1, 1, 1, 1, 1, null));

            Thread.Sleep(900);
        }

        [Test]
        public void It_should_stay_in_leader_state()
        {
            Assert.AreEqual(node.LastFinitState().GetType(), typeof(Leader));
        }

        [Test]
        public void It_should_ignore_append_entry_command()
        {
            Assert.AreEqual(0, bus.MessageCount());
        }
    }
}