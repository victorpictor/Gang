using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Candidate : FinitState
    {
        private Node node;

        private Dictionary<int, long> Granted = new Dictionary<int, long>();

        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            var state = node.GetState();
            if (appendEntries.Term > state.Term)
            {
                Transition(t=>node.Next(new Follower()));
            }

            return new MessageResponse(false, n => { });
        }

        public override MessageResponse Receive(RequestedVote requestedVote)
        {
            return new MessageResponse(false, n => { });
        }

        public override MessageResponse Receive(VoteGranted voteGranted)
        {
            var settings = node.GetSettings();
            if (!Granted.ContainsKey(voteGranted.VoterId))
            {
                Granted.Add(voteGranted.VoterId, voteGranted.Term);
                if (settings.Majority <= Granted.Count)
                    return new MessageResponse(true, n =>n.Next(new Leader()));
                
            }
            return new MessageResponse(false, n => { });
        }
    }
}