using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.States;
using Core.States.TheFollower;
using NUnit.Framework;

namespace Core.Specs.WhenFollowing.AndReceivingRequestedVote
{
    public class MyFollower : Follower
    {
        public Dictionary<long,int> GetVotes()
        {
            return votes;
        }
    }
    
    [TestFixture]
    public class WithHigherTerm : Specification
    {
        private MyFollower state;
        private Node node;

        private InMemoryBus bus;
        private InMemoryBus inMemoryBus;

        public override void Given()
        {

            state = new MyFollower();

            bus = new InMemoryBus();
            inMemoryBus = new InMemoryBus();

            DomainRegistry
                .RegisterService(
                    new NodeLogEntriesService(
                        new PersistentNodeState()
                        {
                            NodeId = 1,
                            Term = 4,
                            EntryIndex = 0,
                            LogEntries = new List<LogEntry>()
                        }));

            node = new Node(new NodeSettings() { NodeId = 1, NodeName = "N1", ElectionTimeout = 50000, Majority = 3 },
                            state,
                            bus,
                            inMemoryBus
                );
        }

        public override void When()
        {
            node.Start();

            inMemoryBus.Send(new RequestedVote() { Term = 4, CandidateId = 2, LastLogIndex = 3, LastLogTerm = 4 });
            Thread.Sleep(1500);

            node.Stop();
        }

        [Test]
        public void It_should_registe_vote_for_term_and_candidate()
        {
            Assert.AreEqual(2, state.GetVotes()[4]);
        }

        [Test]
        public void It_publish_vote_granted()
        {
            Assert.AreEqual(typeof(VoteGranted), bus.messages.Peek().GetType());
        }

        [Test]
        public void It_should_publish_one_message()
        {
            var controllMessage = bus.messages.Dequeue();
            Assert.AreEqual(1, bus.messages.Count);
        }
    }
}