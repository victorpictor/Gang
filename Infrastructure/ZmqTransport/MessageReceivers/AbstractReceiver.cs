using System.Collections;
using System.Threading;
using Core.Messages;
using Core.Messages.Control;

namespace ZmqTransport.MessageReceivers
{
    public abstract class AbstractReceiver
    {
        protected Queue messageQueue = Queue.Synchronized(new Queue());

        public IMessage Receive()
        {
            var message = new NoMessageInQueue();

            var tries = 0;

            lock (messageQueue.SyncRoot)
            {
                while (messageQueue.Count == 0 && tries == 2)
                {
                    Thread.Sleep(100);
                    tries++;
                }

                if (messageQueue.Count != 0)
                    return (IMessage)messageQueue.Dequeue();
            }

            return message;
        }
    }
}