using System.Collections.Generic;
using System.Linq;
using Core.Clustering;
using Core.Messages;
using Core.Receivers;

namespace ZmqTransport.MessageReceivers
{
    public class NodeMessageSubscriber : AbstractReceiver, IReceiveMessagesService
    {
        private List<MessageConsumer> subscriberTasks;

        public NodeMessageSubscriber(NodeSettings settings)
        {
            subscriberTasks = settings
                .ClusterNodes
                .Where(cn => cn.Id != settings.NodeId)
                .Select(n => new MessageConsumer(n.SubscriberPort, messageQueue, settings.NodeId)).ToList();

            subscriberTasks.ForEach(st => st.StartConsume());
        }

        public void StartService()
        {
        }

        public void StopService()
        {
            subscriberTasks.ForEach(st => st.StopConsume());
        }
    }
}