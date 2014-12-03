namespace Core.Messages
{
    public class EntriesAppended : IMessage
    {
        public long Term { get; set; }
        public long LogIndex { get; set; }
        public long NodeId { get; set; }
    }
}