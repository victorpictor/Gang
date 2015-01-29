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

           var logEntriesService = 
                    new NodeLogEntriesService(
                        new PersistentNodeState()
                        {
                            NodeId = 1,
                            Term = 3,
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

            bus.Send(new AppendEntries(1, 1, 1, 1, 1, null));
           
            node.Stop();

            Thread.Sleep(900);
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