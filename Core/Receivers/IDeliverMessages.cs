using Core.Messages;

namespace Core.Receivers
{
    public interface IDeliverMessages
    {
        void Deliver(IMessage message);
    }
}