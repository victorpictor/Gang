using Core.Messages;

namespace Core.Receivers
{
    public interface IReceiveMessages : IReceiveMessages<IMessage>
    {
        IMessage Receive();
    }

    public interface IReceiveMessages<T>
    {
        T Receive();
    }
}