namespace Core.Messages
{
    public class VoteGranted : IReply
    {
        public VoteGranted(int voter, int candidate, long term)
        {
            VoterId = voter;
            To = candidate;
            Term = term;
        }

        public int To { get; set; }
        public int VoterId { get; set; }
        public long Term { get; set; }
    }
}