using Core.Messages;

namespace Core.Receivers
{
    public interface IReceiveMessages : IReceiveMessages<IMessage>
    {
    }

    public interface IReceiveMessages<T>
    {
        T Receive();
    }
}