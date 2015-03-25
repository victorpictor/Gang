﻿using System;
using System.Collections;
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

        public MessageConsumer(ClusterNode clusterNode, Queue internalQueue, int nodeId)
        {
            Action receiverProcess = () =>
            {
                var messages = new MessageRegistry();

                using (var context = NetMQContext.Create())
                using (var subSocket = context.CreateSubscriberSocket())
                {
                    subSocket.Options.ReceiveHighWatermark = 1000;
                    subSocket.Connect(string.Format("tcp://localhost:{0}", clusterNode.SubscriberPort));
                    subSocket.Subscribe(string.Format("node{0}",nodeId));

                    while (true)
                    {
                        var messageTopicReceived = subSocket.ReceiveString();
                        var message = subSocket.ReceiveString();

                        var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(message);

                        Console.WriteLine("{0} Message received on  topic {1} port {2}", DateTime.Now, messageTopicReceived, clusterNode.SubscriberPort);

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