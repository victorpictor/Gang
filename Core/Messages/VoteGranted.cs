namespace Core.Messages
{
    public class VoteGranted : IMessage
    {
        public int CandidateId { get; set; }
        public int VoterId { get; set; }
        public long Term { get; set; }
    }
}