using System;
using Core.Messages;
using Core.Receivers;

namespace Core.Senders
{
    public class ContolMessageSender : ISendMessages
    {
        private IDeliver<IMessage> bus;

        public ContolMessageSender(IDeliver<IMessage> bus)
        {
            this.bus = bus;
        }

        public void Send(IMessage m)
        {
            bus.Deliver(m);
        }

        public void Reply(IReply m)
        {
            bus.Deliver(m);
        }
    }
}