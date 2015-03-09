using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.Receivers;
using Core.States.Services;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.MessageReceivers
{
    public class SubscriberNodeMessageReceiver : IReceiveMessages
    {
        private NodeSettings settings;
       
        private Queue messageQueue = new Queue();

        public SubscriberNodeMessageReceiver(NodeSettings settings)
        {
            this.settings = settings;
            Action receiverProcess = () =>
            {
                var messages = new MessageRegistry();

                using (var context = NetMQContext.Create())
                using (var subSocket = context.CreateSubscriberSocket())
                {
                    subSocket.Options.ReceiveHighWatermark = 1000;
                    subSocket.Connect("tcp://localhost:12345");
                    subSocket.Subscribe("nodetopic"+settings.NodeId);
                    int i = 1;
                    while (true)
                    {
                        var messageTopicReceived = subSocket.ReceiveString();
                        var message = subSocket.ReceiveString();

                        var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(message);

                        messageQueue.Enqueue(
                            (IMessage)messages.Create(envelope.MessageName, envelope.Message.ToString()));
                    }
                }
            };

            new ServiceReference(receiverProcess).StartService();
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