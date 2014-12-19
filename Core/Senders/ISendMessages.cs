using Core.Messages;

namespace Core.Senders
{
    public interface ISend<T>
    {
        void Send(T m);
    }
    
    public interface ISendMessages:ISend<IMessage>
    {
    }
}