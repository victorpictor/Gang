using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Candidate : FinitState
    {
        public void EnterState(ref LogState logState, Node node)
        {
            throw new System.NotImplementedException();
        }

        public void Receive(AppendEntries appendEntries)
        {
            throw new System.NotImplementedException();
        }

        public void Receive(RequestedVote requestedVote)
        {
            throw new System.NotImplementedException();
        }

        public void Receive(VoteGranted voteGranted)
        {
            throw new System.NotImplementedException();
        }
    }
}