using System;
using System.Collections;
using Core.States.Services;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.MessageReceivers
{
    public class MessageConsumer
    {
        public ServiceReference ConsumerService = new ServiceReference(() => { });

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

                    while (!ConsumerService.IsServiceShuttingDown())
                    {
                        var messageTopicReceived = subSocket.ReceiveString(new TimeSpan(0,0,0,2));
                        var message = subSocket.ReceiveString();

                        if (string.IsNullOrWhiteSpace(messageTopicReceived))
                            continue;

                        var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(message);
                      
                        var msg = messages.Create(envelope.MessageName, envelope.Message);

                        internalQueue.Enqueue(msg);
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