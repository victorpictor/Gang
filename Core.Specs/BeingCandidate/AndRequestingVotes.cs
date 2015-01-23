﻿using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.States;
using Core.States.TheCandidate;
using NUnit.Framework;

namespace Core.Specs.BeingCandidate
{
    [TestFixture]
    public class AndRequestingVotes : Specification
    {
        private Candidate state;
        private Node node;

        private InMemoryBus bus;
        private InMemoryBus bus2;

        public override void Given()
        {
            state = new Candidate();

            bus = new InMemoryBus();
            bus2 = new InMemoryBus();

            var logEntriesService = 
                   new NodeLogEntriesService(
                       new PersistentNodeState()
                       {
                           NodeId = 1,
                           Term = 1,
                           EntryIndex = 0,
                           LogEntries = new List<LogEntry>()
                       });
           
            var registry = new DomainRegistry()
                .UseDomainMessageSender(bus)
                .UseNodeMessageSender(bus2)
                .UseNodeLogEntriesService(logEntriesService)
                .UseNodeSettings(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 1000, Majority = 3 });

            node = new Node(state,registry,bus);
        }


        public override void When()
        {
            node.Start();
            Thread.Sleep(3000);
        }

        [Test]
        public void It_should_stay_Candidate()
        {
            Assert.AreEqual(typeof(Candidate), node.LastFinitState().GetType());
        }

        [Test]
        public void It_should_publish_at_least_3_times_the_request_vote_message()
        {
            Assert.Greater(bus2.messages.Count,3);
        }
    }
}