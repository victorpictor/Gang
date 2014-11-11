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
        private DateTime lastMessageReceivedOn;
        private Dictionary<long, int> votes = new Dictionary<long, int>();


        public override void EnterState(ref PersistentNodeState persistentNodeState, Node node)
        {
            Timer();
            base.EnterState(ref persistentNodeState, node);
        }

        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            var state = node.GetState();

            if (state.Term > appendEntries.Term)
                return new MessageResponse(false, n => { });

            lastMessageReceivedOn = DateTime.Now;

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

        public MessageResponse Receive(TimedOut timedOut)
        {
            return new MessageResponse(true, n => n.Next(new Candidate()));
        }

        public virtual IMessage NextMessage()
        {
            return new TimedOut();
        }

        private void Timer()
        {
            Task.Factory.StartNew(() =>
            {
                var settigs = node.GetSettings();

                while (DateTime.Now.Subtract(lastMessageReceivedOn).Milliseconds < settigs.ElectionTimeout) ;
                                //send time out;
            });
        }
    }
}