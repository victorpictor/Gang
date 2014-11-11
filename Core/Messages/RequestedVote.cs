namespace Core.Messages
{
    public class RequestedVote : IMessage
    {
        public int CandidateId { get; set; }
        public long LastLogTerm { get; set; }
        public long LastLogIndex { get; set; }
    }
}