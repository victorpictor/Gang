namespace Core.Messages.Control
{
    public class TimedOut : IMessage
    {
        public TimedOut(int nodeId, long term)
        {
            this.NodeId = nodeId;
            this.Term = term;
        }

        public long NodeId { get; set; }
        public long Term { get; set; }
    }
}