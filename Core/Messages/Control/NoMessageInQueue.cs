namespace Core.Messages.Control
{
    public class NoMessageInQueue:IMessage
    {
        public NoMessageInQueue()
        {
            Term = 0;
        }

        public long Term { get; set; }
    }
}