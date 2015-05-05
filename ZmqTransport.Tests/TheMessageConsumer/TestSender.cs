using Core.Clustering;
using Core.Messages;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace ZmqTransport.Tests.Consumer
{
    public class TestSender
    {
        private NodeSettings settings;

        private NetMQContext context;
        private PublisherSocket pubSocket;


        public TestSender(NodeSettings settings)
        {
            this.settings = settings;

            context = NetMQContext.Create();
            pubSocket = context.CreatePublisherSocket();

            pubSocket.Options.SendHighWatermark = 1000;
            pubSocket.Bind(string.Format("tcp://localhost:{0}", settings.SubscribersPort));
        }

        public void Send(IMessage m)
        {
            settings.ClusterNodes.ForEach(s =>
                {

                    if (settings.NodeId != s.Id)
                        pubSocket.SendMore(string.Format("node{0}", s.Id)).Send(
                            JsonConvert.SerializeObject(new MessageEnvelope()
                                {
                                    SenderId = settings.NodeId,
                                    MessageName = m.GetType().Name,
                                    Message = JsonConvert.SerializeObject(m)
                                }));
                }
                );
        }

        ~TestSender()
        {
            pubSocket.Dispose();
            context.Dispose();
        }
    }
}