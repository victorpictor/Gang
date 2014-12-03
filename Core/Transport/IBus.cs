using Core.Receivers;
using Core.Senders;

namespace Core.Transport
{
    public interface IBus : IReceiveMessages, ISendMessages
    {
    }
}