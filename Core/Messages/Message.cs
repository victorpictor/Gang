namespace Core.Messages
{

    public interface IMessage
    {
    }

    public interface IRequest : IMessage
    {
        long Term { get; set; }
    }

    public interface IEvent : IMessage
    {
        long Term { get; set; }
    }

    public interface IReply: IMessage
    {
        int To { get; set; }
    }
}