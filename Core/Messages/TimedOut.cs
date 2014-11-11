namespace Core.Messages
{
    public class TimedOut : IMessage
    {
        public long CurrentTerm { get; set; }
        public long NodeId { get; set; }
    }
}