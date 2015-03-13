using Core.Clustering;
using Core.Messages;
using Core.Senders;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace ZmqTransport.MessageSenders
{
    public class NodeMessageSender : ISendMessages
    {
        private NodeSettings settings;

        private NetMQContext context;
        private PublisherSocket pubSocket;


        public NodeMessageSender(NodeSettings settings)
        {
            this.settings = settings;
            
            context = NetMQContext.Create();
            pubSocket = context.CreatePublisherSocket();

            pubSocket.Options.SendHighWatermark = 1000;
            pubSocket.Bind(string.Format("tcp://localhost:{0}", settings.SubscribersPort));
        }

        public void Send(IMessage m)
        {
            pubSocket.SendMore("all").Send(
                         JsonConvert.SerializeObject(new MessageEnvelope()
                             {
                                 SenderId = settings.NodeId,
                                 MessageName = m.GetType().Name,
                                 Message = m
                             }));
        }

        ~NodeMessageSender()
        {
            pubSocket.Dispose();
            context.Dispose();
        }
    }
}