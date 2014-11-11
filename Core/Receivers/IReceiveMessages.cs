using Core.Messages;

namespace Core.Receivers
{
    public interface IReceiveMessages
    {
        IMessage Receive();
    }
}