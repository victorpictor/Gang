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
            pubSocket.Bind("tcp://localhost:52345");
        }

        public void Send(IMessage m)
        {

               settings.ClusterNodes.ForEach(n =>
                 {
                     if (n != settings.NodeId)
                     {
                         pubSocket.SendMore("nodetopic" + n.ToString())
                                  .Send(JsonConvert.SerializeObject(new MessageEnvelope() { SenderId = settings.NodeId, MessageName = m.GetType().Name, Message = m }));
                         Console.WriteLine("Sent {0} to {1} term {2}", m.GetType().Name, "nodetopic" + n.ToString(), m.Term);
                     }
                 });
            
        }

        public void Reply(IMessage m)
        {
            using (var context = NetMQContext.Create())
            using (var pubSocket = context.CreatePublisherSocket())
            {
                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind("tcp://localhost:12345");

                pubSocket.SendMore("all").Send(JsonConvert.SerializeObject(new MessageEnvelope() { SenderId = settings.NodeId, MessageName = m.GetType().Name, Message = m }));
            }
        }

        ~NodeMessageSender()
        {
            pubSocket.Dispose();
            context.Dispose();
        }
    }
}