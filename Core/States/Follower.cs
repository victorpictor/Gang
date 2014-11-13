using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Follower : FinitState
    {
        protected DateTime lastReceivedOn = DateTime.Now;
        protected Dictionary<long, int> votes = new Dictionary<long, int>();
        
        public override void EnterState(ref PersistentNodeState persistentNodeState, Node node)
        {
            base.node = node;

            Timer();
            base.EnterState(ref persistentNodeState, node);
        }

        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            var state = node.GetState();

            if (appendEntries.Term < state.Term)
                return new MessageResponse(false, () => { });

            if (appendEntries.Term > state.Term)
                state.Term = appendEntries.Term;

            lastReceivedOn = DateTime.Now;

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
                    node.Next(new Candidate());
                });
        }
        
        private void Timer()
        {
            var timer = new Thread(() =>
            {
                var settigs = node.GetSettings();
                var state = node.GetState();

                while (DateTime.Now.Subtract(lastReceivedOn).TotalMilliseconds < settigs.ElectionTimeout){}

                node.Send(new TimedOut() { NodeId = state.NodeId, Term = state.Term });
            });

            parallelTasks.Add(timer);
            timer.Start();
         }
    }
}