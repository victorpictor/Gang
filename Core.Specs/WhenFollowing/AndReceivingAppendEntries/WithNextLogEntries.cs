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
    public class WithNextLogEntries : Specification
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
                            EntryIndex = 1,
                            LogEntries = new List<LogEntry>(),
                            PendingCommit = new LogEntry() { Term = 2, Index = 1, MachineCommands = new List<object>() { new object() } }
                        });

            node = new Node(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 10000, Majority = 3 },
                            state,
                            logEntriesService,
                            bus,
                            bus
                );
        }

        public override void When()
        {
            bus.Send(new AppendEntries() { Term = 2, LogIndex = 2, PrevTerm = 2, PrevLogIndex = 1, MachineCommands = new List<object>(){new object()} });

            node.Start();
            Thread.Sleep(1500);
            node.Stop();
        }


        [Test]
        public void It_should_append_new_entries()
        {
            Assert.AreEqual(2, node.NodLogEntriesService().NodeState().EntryIndex);
            Assert.AreEqual(2, node.NodLogEntriesService().NodeState().Term);
        }

        [Test]
        public void It_should_not_change_node_state()
        {
            Assert.AreEqual(node.LastFinitState().GetType(), typeof(Follower));
        }

    }
}