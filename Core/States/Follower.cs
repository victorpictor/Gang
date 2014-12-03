using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Follower : FinitState
    {
        protected DateTime lastReceivedOn = DateTime.Now;
        protected Dictionary<long, int> votes = new Dictionary<long, int>();
        
        public override void EnterState(Node node)
        {
            base.node = node;

            Timer();
            base.EnterState(node);
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
        
        private void Timer()
        {
            var timer = new Thread(() =>
            {
                var settings = node.GetSettings();
                var state = node.GetState();
                var started = DateTime.Now;
                
                while (DateTime.Now.Subtract(started).TotalMilliseconds <= settings.ElectionTimeout)
                {
                }

                node.Send(new TimedOut() { NodeId = state.NodeId, Term = state.Term });
            });

            parallelTasks.Add(timer);
            timer.Start();
         }
    }
}