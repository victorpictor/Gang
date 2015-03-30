using System;
using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;
using Core.Messages.Control;

namespace Core.States.TheFollower
{
    public class Follower : FinitState
    {
        protected Dictionary<long, int> votes = new Dictionary<long, int>();

        public Follower(){}
        public Follower(Dictionary<long, int> votes)
        {
            this.votes = votes;
        }


        public override void EnterNewState(Node node)
        {
            this.Info(string.Format("Node is {0}", this.GetType().Name));

            base.node = node;

            RegisterService(
                new TimeoutService(node)
                    .Reference());

            RegisterService(
                new LogRecoveryService()
                    .Reference());

            StartRegisteredServices();
        }

        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            this.Info(string.Format("Received AppendEntries from node {0}, term {1} log index {2}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex));

            var logEntryService = node
                .GetRegistry()
                .LogEntriesService();

            var state = logEntryService.NodeState();

            if (appendEntries.Term < state.Term)
                return new MessageResponse(false, () => { });

            if (appendEntries.Term > state.Term)
                logEntryService.UpdateTerm(appendEntries.Term);

            logEntryService.MessageReceivedNow();

            if (!appendEntries.IsHeartBeat())
            {
                logEntryService
                    .Append(appendEntries.Term,
                            appendEntries.LogIndex,
                            appendEntries.PrevTerm,
                            appendEntries.PrevLogIndex,
                            appendEntries.MachineCommands);

                node
                    .GetRegistry()
                    .NodeMessageSender()
                    .Reply(new EntriesAppended()
                        {
                            To = appendEntries.LeaderId,
                            Term = appendEntries.Term,
                            LogIndex = appendEntries.LogIndex,
                            NodeId = node.GetRegistry().NodeSettings().NodeId
                        });

                this.Info(string.Format("AppendedEntries from node {0}, term {1} log index {2}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex));
            }

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(RequestedVote requestedVote)
        {
            var state = node.GetRegistry().LogEntriesService().NodeState();

            this.Info(string.Format("Requested vote, candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term));
            
            if (state.Term < requestedVote.LastLogTerm)
                return new MessageResponse(false, () => { });

            if (state.Term < requestedVote.Term)
            {
                if (!votes.ContainsKey(requestedVote.Term))
                {
                    votes.Add(requestedVote.Term, requestedVote.CandidateId);

                    this.Info(string.Format("Vote granted for {0}, term {1}", requestedVote.CandidateId, requestedVote.Term));

                    node
                        .GetRegistry()
                        .LogEntriesService().MessageReceivedNow();

                    node.GetRegistry()
                        .NodeMessageSender()
                        .Reply(new VoteGranted(state.NodeId, requestedVote.CandidateId, requestedVote.Term));

                }
                else
                {
                    this.Info(string.Format("Already voted in this term for candidate {0}, term {1}, my term was {2}",
                        requestedVote.CandidateId, requestedVote.Term, state.Term));
                }
            }
            else
            {
                this.Info(string.Format("Did not grant vote for candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term));
            }

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(TimedOut timedOut)
        {
            this.Info("Timed out");
            return new MessageResponse(true, () =>
                {
                    node
                        .GetRegistry()
                        .LogEntriesService()
                        .IncrementTerm();
                    StopRegisteredServices();
                    node.Next(new StateFactory().Candidate());
                });
        }

    }
}