using System;
using System.Collections.Generic;
using Core.Messages;
using Core.Receivers;
using Core.Senders;
using Core.States.Services;

namespace Core.Specs
{
    public class InMemoryBus : ISendMessages, IReceiveMessagesService
    {
        public Queue<IMessage> messages = new Queue<IMessage>();
        public ServiceReference ServiceReference { get; set; }

        public void Send(IMessage m)
        {
            messages.Enqueue(m);
        }

        public void Reply(IReply m)
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

        public void StartService()
        {
        }

        public void StopService()
        {
        }
    }
    
}