using System;
using Core.Messages;
using Core.Receivers;

namespace Core.Senders
{
    public class ContolMessageSender : ISendMessages
    {
        private IDeliverMessages bus;

        public ContolMessageSender(IDeliverMessages bus)
        {
            this.bus = bus;
        }

        public void Send(IMessage m)
        {
            Console.WriteLine("{0} Sending {1}", DateTime.Now, m.GetType().Name);

            bus.Deliver(m);
        }
    }
}