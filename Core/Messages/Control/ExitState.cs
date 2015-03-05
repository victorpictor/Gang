namespace Core.Messages.Control
{
    public class ExitState : IMessage
    {
        public long Term { get; set; }
    }
}