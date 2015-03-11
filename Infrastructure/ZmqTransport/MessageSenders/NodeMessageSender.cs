using System;
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

               settings.ClusterNodes.ForEach(n =>
                 {
                     if (n.Id != settings.NodeId)
                     {
                         pubSocket.SendMore("nodetopic" + n.ToString())
                                  .Send(JsonConvert.SerializeObject(new MessageEnvelope("nodetopic" + n.Id.ToString()) { SenderId = settings.NodeId, MessageName = m.GetType().Name, Message = m }));
                         Console.WriteLine("Sent {0} to {1} term {2}", m.GetType().Name, "nodetopic" + n.Id.ToString(), m.Term);
                     }
                 });
            
        }

        ~NodeMessageSender()
        {
            pubSocket.Dispose();
            context.Dispose();
        }
    }
}