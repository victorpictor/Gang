namespace Core.Messages
{
    public class EntriesAppended : IReply
    {
        public long Term { get; set; }
        public long LogIndex { get; set; }
        public long NodeId { get; set; }
        public int To { get; set; }
    }
}