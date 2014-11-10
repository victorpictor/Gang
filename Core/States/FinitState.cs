using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public interface FinitState
    {
        void EnterState(ref LogState logState, Node node);

        void Receive(AppendEntries appendEntries);
        void Receive(RequestedVote requestedVote);
        void Receive(VoteGranted voteGranted);
    }
}