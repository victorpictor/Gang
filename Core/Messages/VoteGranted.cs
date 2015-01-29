namespace Core.Messages
{
    public class VoteGranted : IMessage
    {
        public VoteGranted(int voter, int candidate, long term)
        {
            VoterId = voter;
            CandidateId = candidate;
            Term = term;
        }

        public int CandidateId { get; set; }
        public int VoterId { get; set; }
        public long Term { get; set; }
    }
}