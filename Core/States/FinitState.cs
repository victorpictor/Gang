using Core.Messages;

namespace Core.States
{
    public interface FinitState
    {
        void EnterState();

        void Receive(AppendEntries appendEntries);
        void Receive(RequestedVote requestedVote);
        void Receive(VoteGranted voteGranted);
    }
}