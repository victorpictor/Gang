namespace Core.Messages
{
    public class ExitState : IMessage
    {
        public long Term { get; set; }
    }
}