namespace Core.Messages
{
    public interface IMessage
    {
        long Term { get; set; }
    }

    public interface IReply: IMessage
    {
        int To { get; set; }
    }
}