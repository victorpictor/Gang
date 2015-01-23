using System.Collections.Generic;
using Core.Messages;
using Core.Receivers;
using Core.Senders;

namespace Core.Specs
{
    public class InMemoryBus: ISendMessages, IReceiveMessages
    {
        public Queue<IMessage> messages = new Queue<IMessage>();

        public void Send(IMessage m)
        {
            messages.Enqueue(m);
        }

        public IMessage Receive()
        {
            if (messages.Count > 0)
                return messages.Dequeue();

            return new AppendEntries(-1,-1,null);
        }

        public int MessageCount()
        {
            return messages.Count;
        }
    }

}