namespace Core.Messages
{
    public class AppendEntries : IMessage
    {
        public int LeaderId { get; set; }
        public long Term { get; set; }
        public long PrevLogIndex { get; set; }
        public long CommitIndex { get; set; }
    }
}