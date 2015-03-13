namespace Core.Messages.Control
{
    public class NoMessageInQueue:IMessage
    {
        public NoMessageInQueue()
        {
            Term = -1;
        }

        public long Term { get; set; }
    }
}