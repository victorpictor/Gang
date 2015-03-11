using System;
using System.Collections;
using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.MessageReceivers
{
    public class MessageConsumer
    {
        public ServiceReference ConsumerService;

        public MessageConsumer(ClusterNode clusterNode, Queue internalQueue)
        {
            Action receiverProcess = () =>
            {
                var messages = new MessageRegistry();

                using (var context = NetMQContext.Create())
                using (var subSocket = context.CreateSubscriberSocket())
                {
                    subSocket.Options.ReceiveHighWatermark = 1000;
                    subSocket.Connect(string.Format("tcp://localhost:{0}", clusterNode.SubscriberPort));
                    subSocket.Subscribe("");

                    while (true)
                    {
                        var messageTopicReceived = subSocket.ReceiveString();
                        var message = subSocket.ReceiveString();

                        var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(message);

                        internalQueue.Enqueue(
                            (IMessage)messages.Create(envelope.MessageName, envelope.Message.ToString()));
                    }
                }
            };

            ConsumerService = new ServiceReference(receiverProcess);
        }

        public void Consume()
        {
           ConsumerService.StartService();
        }
        
        public void StopConsumer()
        {
            ConsumerService.StopService();
        }
    }
}