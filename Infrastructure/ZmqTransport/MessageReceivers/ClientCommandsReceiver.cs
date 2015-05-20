using Core.Clustering;
using Core.Receivers;
using ZmqTransport.Settings;

namespace ZmqTransport.MessageReceivers
{
    public class ClientCommandsReceiver : AbstractReceiver, IReceiveMessagesService
    {
        private MessageConsumer consumer;
       
        public ClientCommandsReceiver(NodeSettings nodeSettings, ClientSettigs clientSettigs)
        {
            consumer = new MessageConsumer(clientSettigs.RequestsPort, messageQueue, nodeSettings.NodeId);
        }

        public void StartService()
        {
            consumer.StartConsume();
        }

        public void StopService()
        {
            consumer.StopConsume();
        }
    }
}