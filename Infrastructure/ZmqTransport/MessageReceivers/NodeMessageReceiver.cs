using Core.Messages;
using Core.Receivers;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.MessageReceivers
{
    public class NodeMessageReceiver : IReceiveMessages
    {
        public IMessage Receive()
        {
            var messages = new MessageRegistry();

            using (var context = NetMQContext.Create())
            using (var server = context.CreateResponseSocket())
            {
                server.Bind("tcp://localhost:5556");

                while (true)
                {
                    var message = server.ReceiveString();
                    
                    var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(message);

                    return (IMessage)messages.Create(envelope.MessageName, envelope.Message.ToString());
                }
            }
        }
    }
}