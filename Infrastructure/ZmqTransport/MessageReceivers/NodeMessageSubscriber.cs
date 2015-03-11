using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.Receivers;

namespace ZmqTransport.MessageReceivers
{
    public class NodeMessageSubscriber : IReceiveMessages
    {
        private Queue messageQueue = Queue.Synchronized(new Queue());

        public NodeMessageSubscriber(NodeSettings settings)
        {
            var subscriberTasks = settings
                .ClusterNodes
                .Where(cn => cn.Id != settings.NodeId)
                .Select(n => new MessageConsumer(n, messageQueue)).ToList();

            subscriberTasks.ForEach(st => st.Consume());
        }

        public IMessage Receive()
        {
            IMessage message = new AppendEntries(-1, -1, new List<object>());
            var tries = 0;
            lock (messageQueue.SyncRoot)
            {
                while (messageQueue.Count == 0 && tries == 2)
                {
                    Thread.Sleep(100);
                    tries++;
                }
               
                if(messageQueue.Count != 0)
                message = (IMessage)messageQueue.Dequeue();
            }

            return message;
        }

    }
}