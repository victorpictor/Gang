namespace Core.Messages
{
    public interface IMessage
    {
    }

    public interface IReply: IMessage
    {
        int To { get; set; }
    }
}