namespace Core.Messages
{
    public class RequestedVote : IMessage
    {
        public RequestedVote(int candidateId, long lastLogTerm, long term, long lastLogIndex)
        {
            this.CandidateId = candidateId;
            this.LastLogTerm = lastLogTerm;
            this.Term = term;
            this.LastLogIndex =lastLogIndex;
        }

        public int CandidateId { get; set; }
        public long LastLogTerm { get; set; }
        public long LastLogIndex { get; set; }
        public long Term { get; set; }
    }
}