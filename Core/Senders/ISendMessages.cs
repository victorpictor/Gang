using Core.Messages;

namespace Core.Senders
{
    public interface ISend<T>
    {
        void Reply(IReply m);
        void Send(T m);
    }
    
    public interface ISendMessages:ISend<IMessage>
    {
    }
}