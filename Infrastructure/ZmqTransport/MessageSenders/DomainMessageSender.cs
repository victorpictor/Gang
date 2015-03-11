using Core.Messages;
using Core.Senders;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.MessageSenders
{
    public class DomainMessageSender : ISendMessages
    {
        public void Send(IMessage m)
        {
            using (var context = NetMQContext.Create())
            using (var client = context.CreateRequestSocket())
            {
                client.Connect("tcp://localhost:5556");

                var msg = JsonConvert.SerializeObject(new MessageEnvelope("") { MessageName = m.GetType().Name, Message = m });
                client.Send(msg);
            }
        }
    }
}