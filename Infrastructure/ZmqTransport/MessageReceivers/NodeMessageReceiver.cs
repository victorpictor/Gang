using System;
using System.Collections;
using System.Collections.Generic;
using Core.Messages;
using Core.Receivers;
using Core.States.Services;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.MessageReceivers
{
    public class NodeMessageReceiver : IReceiveMessages
    {
        private Queue messageQueue = new Queue(); 

        public NodeMessageReceiver()
        {
            Action receiverProcess = () =>
                {
                    var messages = new MessageRegistry();

                    using (var context = NetMQContext.Create())
                    using (var server = context.CreateResponseSocket())
                    {

                        server.Bind("tcp://localhost:5556");

                        while (true)
                        {
                            var message = server.ReceiveString();
                            server.Send("Ok");
                            var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(message);

                            messageQueue.Enqueue(
                                (IMessage) messages.Create(envelope.MessageName, envelope.Message.ToString()));
                        }
                    }
                };

            new ServiceReference(receiverProcess).StartService();
        }

        public IMessage Receive()
        {
            IMessage message = new AppendEntries(-1,-1,new List<object>());

            lock (messageQueue.SyncRoot)
            {
                if (messageQueue.Count > 0)
                    message = (IMessage)messageQueue.Dequeue();
            }

            return message;
        }
    }
}