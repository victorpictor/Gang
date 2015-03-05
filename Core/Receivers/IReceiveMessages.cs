using Core.Messages;
using Core.States.Services;

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