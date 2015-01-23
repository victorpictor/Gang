﻿using System.Collections.Generic;
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
                .UseToReceiveMessages(bus)
                .UseNodeLogEntriesService(logEntriesService)
                .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 });

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