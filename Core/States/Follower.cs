using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Clustering;
using Core.Messages;
using Core.Messages.Senders;

namespace Core.States
{
    public class Follower : FinitState
    {
        private DateTime lastReceivedOn = DateTime.Now;
        private Dictionary<long, int> votes = new Dictionary<long, int>();
        
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

            if (state.Term >= requestedVote.LastLogTerm)
                return new MessageResponse(false, () => { });

            if (!votes.ContainsKey(requestedVote.LastLogTerm))
            {
                votes.Add(requestedVote.LastLogTerm,requestedVote.CandidateId);

                new MessageSender().Send(new VoteGranted() { VoterId = state.NodeId, CandidateId = requestedVote.CandidateId, Term = requestedVote.LastLogTerm });
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

        public override IMessage NextMessage()
        {
            return node.Receive();
        }

        private void Timer()
        {
            Task.Factory.StartNew(() =>
            {
                var settigs = node.GetSettings();
                var state = node.GetState();

                while (DateTime.Now.Subtract(lastReceivedOn).TotalMilliseconds < settigs.ElectionTimeout){}

                node.Send(new TimedOut() { NodeId = state.NodeId, CurrentTerm = state.Term });
            });
        }
    }
}