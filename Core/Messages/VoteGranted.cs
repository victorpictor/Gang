namespace Core.Messages
{
    public class VoteGranted : IMessage
    {
        public int CandidateId;
        public int VoterId;
        public long Term;
    }
}