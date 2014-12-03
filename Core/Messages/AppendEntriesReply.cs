namespace Core.Messages
{
    public class AppendEntriesReply : IMessage
    {
        public long Term { get; set; }
    }
}