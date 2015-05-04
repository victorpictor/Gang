using System;
using System.Collections;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.MessageReceivers
{
    internal class MessageConsumer
    {
        public ServiceReference ConsumerService;

        public MessageConsumer(int subscriberPort, Queue internalQueue, int nodeId)
        {
            Action receiverProcess = () =>
            {
                var messages = new MessageRegistry();

                using (var context = NetMQContext.Create())
                using (var subSocket = context.CreateSubscriberSocket())
                {
                    subSocket.Options.ReceiveHighWatermark = 1000;
                    subSocket.Connect(string.Format("tcp://localhost:{0}", subscriberPort));
                    subSocket.Subscribe(string.Format("node{0}",nodeId));

                    while (ConsumerService.IsServiceShuttingDown())
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

        public void StartConsume()
        {
           ConsumerService.StartService();
        }

        public void StopConsume()
        {
            ConsumerService.StopService();
        }
    }
}