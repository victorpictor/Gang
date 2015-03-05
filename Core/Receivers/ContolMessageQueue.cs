using System.Collections.Generic;
using Core.Messages;
using Core.Messages.Control;

namespace Core.Receivers
{
    public class ContolMessageQueue : IReceiveMessages, IDeliverMessages
    {
        private Queue<IMessage> messageQueue = new Queue<IMessage>();

        public void Deliver(IMessage message)
        {
            messageQueue.Enqueue(message);
        }
        
        public IMessage Receive()
        {
            if (messageQueue.Count > 0)
                return messageQueue.Dequeue();

            return new NoMessageInQueue();
        }
    }
}