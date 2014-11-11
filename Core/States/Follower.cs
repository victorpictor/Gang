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

            if (state.Term > appendEntries.Term)
                return new MessageResponse(false, n => { });

            lastReceivedOn = DateTime.Now;

            return new MessageResponse(false, n => { });

        }

        public override MessageResponse Receive(RequestedVote requestedVote)
        {
            var state = node.GetState();

            if (state.Term >= requestedVote.LastLogTerm)
                return new MessageResponse(false, n => { });

            if (!votes.ContainsKey(requestedVote.LastLogTerm))
            {
                votes.Add(requestedVote.LastLogTerm,requestedVote.CandidateId);

                new MessageSender().Send(new VoteGranted() { VoterId = state.NodeId, CandidateId = requestedVote.CandidateId, Term = requestedVote.LastLogTerm });
            }

            return new MessageResponse(false, n => { });
        }

        public override MessageResponse Receive(TimedOut timedOut)
        {
            return new MessageResponse(true, n => n.Next(new Candidate()));
        }

        public override IMessage NextMessage()
        {
            return new TimedOut();
        }

        private void Timer()
        {
            Task.Factory.StartNew(() =>
            {
                var settigs = node.GetSettings();
                var state = node.GetState();

                while (DateTime.Now.Subtract(lastReceivedOn).Milliseconds < settigs.ElectionTimeout) ;
                    new MessageSender().Send(new TimedOut() { NodeId = state.NodeId, CurrentTerm = state.Term });
            });
        }
    }
}