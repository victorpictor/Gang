﻿using System.Collections.Generic;
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
    public class WithNoEntries : Specification
    {
        private Follower state;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {

            state = new Follower();

            bus = new InMemoryBus();

            var logEntryStore = new LogEntryStore();
            logEntryStore.Append(new LogEntry()
            {
                NodeId = 1,
                Term = 2,
                Index = 1,
                MachineCommands = new List<object>()
            });
           
            var registry = new DomainRegistry()
             .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 })
             .UseContolMessageQueue()
             .UseNodeMessageSender(bus)
             .UseLogEntryStore(logEntryStore)
             .UseToReceiveMessages(bus);
            
            node = new Node(state,registry);
        }

        public override void When()
        {
            bus.Send(new AppendEntries(1, 2, 2, 2, 1, new List<object>()));

            node.Start();
            Thread.Sleep(500); 
            node.Stop();
            Thread.Sleep(300);
        }


        [Test]
        public void It_should_not_append_new_entries()
        {
            Assert.AreEqual(1, node.GetRegistry().LogEntriesService().NodeState().EntryIndex);
            Assert.AreEqual(2, node.GetRegistry().LogEntriesService().NodeState().Term);
        }

        [Test]
        public void It_should_not_change_node_state()
        {
            Assert.AreEqual(node.LastFinitState().GetType(), typeof(Follower));
        }

    }
}