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
    public class WithLowerTerm : Specification
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
                Term = 3,
                Index = 0,
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
            node.Start();

            bus.Send(new AppendEntries(1, 1, 1, 1, 1, null));
            
            Thread.Sleep(900);
            node.Stop();
            Thread.Sleep(300);
        }

        [Test]
        public void It_should_stay_in_follower_state()
        {
            Assert.AreEqual(typeof(Follower), node.LastFinitState().GetType());
        }

        [Test]
        public void It_should_ignore_append_entry_command()
        {
            Assert.AreEqual(0, bus.MessageCount());
        }
    }
}