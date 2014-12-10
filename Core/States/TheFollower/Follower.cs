using System;
using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;

namespace Core.States.TheFollower
{
    public class Follower : FinitState
    {
        protected DateTime lastReceivedOn = DateTime.Now;
        protected Dictionary<long, int> votes = new Dictionary<long, int>();
        
        public override void EnterState(Node node)
        {
            base.node = node;

            base.EnterState(node);

            RegisterService(
                new TimeoutService(base.node)
                    .StartService().Reference());

            StartRegisteredServices();
        }

        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            var state = node.GetState();

            if (appendEntries.Term < state.Term)
                return new MessageResponse(false, () => { });

            if (appendEntries.Term > state.Term)
                state.Term = appendEntries.Term;

            lastReceivedOn = DateTime.Now;

            if (!appendEntries.IsHeartBeat())
            {
                state.Append(appendEntries.Term, appendEntries.LogIndex, appendEntries.PrevTerm,appendEntries.PrevLogIndex, appendEntries.MachineCommands);
                node.Send(new EntriesAppended() { Term = appendEntries.Term, LogIndex = appendEntries.LogIndex, NodeId = node.GetSettings().NodeId});
            }

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(RequestedVote requestedVote)
        {
            var state = node.GetState();

            if (state.Term < requestedVote.LastLogTerm)
                return new MessageResponse(false, () => { });

            if (state.Term == requestedVote.LastLogTerm)
            if (!votes.ContainsKey(requestedVote.LastLogTerm))
            {
                votes.Add(requestedVote.LastLogTerm,requestedVote.CandidateId);

                node.Send(new VoteGranted() { VoterId = state.NodeId, CandidateId = requestedVote.CandidateId, Term = requestedVote.LastLogTerm });
            }

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(TimedOut timedOut)
        {
            return new MessageResponse(true, () =>
                {
                    var state = node.GetState();
                    state.Term++;
                    node.Next(Candidate());
                });
        }
        
    }
}