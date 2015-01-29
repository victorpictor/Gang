﻿using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States.TheFollower;
using NUnit.Framework;

namespace Core.Specs.WhenFollowing.AndReceivingAppendEntries
{
    [TestFixture]
    public class WithHigherTerm : Specification
    {
        private Follower state;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {

            state = new Follower();

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
               .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 20000, Majority = 3 });

            node = new Node(state,registry);
        }

        public override void When()
        {
            node.Start();

            bus.Send(new AppendEntries(1, 4, 4, 3, 2, new List<object>() {new object()}));
            Thread.Sleep(900);
            node.Stop();
        }

        [Test]
        public void It_should_not_ignore_append_entry_command()
        {
            var controllMessage = bus.messages.Dequeue();
            Assert.AreEqual(0, bus.MessageCount());
        }

        [Test]
        public void It_should_update_term()
        {
            Assert.AreEqual(4, node.GetRegistry().LogEntriesService().NodeState().Term);
        }

        [Test]
        public void It_should_stay_in_follower_state()
        {
            Assert.AreEqual(typeof(Follower), node.LastFinitState().GetType());
        }
    }
}