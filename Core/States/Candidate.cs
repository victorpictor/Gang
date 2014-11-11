using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Candidate : FinitState
    {
        private Node node;

        private HashSet<int> Granted = new HashSet<int>();

        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            var state = node.GetState();
            
            if (appendEntries.Term > state.Term)
                return new MessageResponse(true, () => node.Next(new Follower()));

          return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(VoteGranted voteGranted)
        {
            var settings = node.GetSettings();

            Granted.Add(voteGranted.VoterId);

            if (Granted.Count >= settings.Majority)
                return new MessageResponse(true, () => node.Next(new Leader()));

            return new MessageResponse(false, () => { });
        }
    }
}