using Core.Messages;

namespace Core.Senders
{
    public interface ISendMessages
    {
        void Send(IMessage m);
    }
}