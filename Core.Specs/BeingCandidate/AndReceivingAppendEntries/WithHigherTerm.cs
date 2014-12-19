﻿using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States;
using Core.States.TheCandidate;
using Core.States.TheFollower;
using NUnit.Framework;

namespace Core.Specs.BeingCandidate.AndReceivingAppendEntries
{
    [TestFixture]
    public class WithHigherTerm : Specification
    {
        private Candidate state;
        private Node node;

        private InMemoryBus bus;

        public override void Given()
        {

            state = new Candidate();

            bus = new InMemoryBus();

            node = new Node(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 },
                            new PersistentNodeState()
                            {
                                NodeId = 1,
                                Term = 2,
                                EntryIndex = 0,
                                LogEntries = new List<LogEntry>()
                            },
                            state,
                            bus,
                            bus
                );
        }

        public override void When()
        {
            node.Start();

            bus.Send(new AppendEntries() { Term = 4 });

            node.Stop();

            Thread.Sleep(900);
        }

        [Test]
        public void It_should_update_term()
        {
            Assert.AreEqual(node.GetState().Term, 4);
        }

        [Test]
        public void It_should_become_follower()
        {
            Assert.AreEqual(node.LastFinitState().GetType(), typeof(Follower));
        }
    }
}