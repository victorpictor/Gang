using System;
using System.Collections.Generic;
using Core.Messages;
using Core.Messages.Control;
using Core.Receivers;
using Core.Senders;
using Core.States.Services;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace ZmqTransport.MessageSenders
{
    public class ControlMessageBus : IReceiveMessages, ISendMessages
    {
        private ServiceReference controlMessageConsumer;

        private NetMQContext context;
        private PairSocket sender;
        private PairSocket receiver;

        private Queue<IMessage> messageQueue = new Queue<IMessage>();

        public ControlMessageBus()
        {

            context = NetMQContext.Create();
            sender = context.CreatePairSocket();
            receiver = context.CreatePairSocket();

            sender.Bind("inproc://inproc-demo");
            receiver.Connect("inproc://inproc-demo");

            Action receiverProcess = () =>
                {
                    var messages = new MessageRegistry();

                    while (true)
                    {
                        var message = receiver.ReceiveString();

                        var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(message);

                        messageQueue.Enqueue(
                            (IMessage)messages.Create(envelope.MessageName, envelope.Message.ToString()));
                    }
                };

            controlMessageConsumer = new ServiceReference(receiverProcess);
            controlMessageConsumer.StartService();
        }

        public IMessage Receive()
        {
            if (messageQueue.Count > 0)
                return messageQueue.Dequeue();

            return new NoMessageInQueue();
        }

        public void Send(IMessage message)
        {
            sender.Send(JsonConvert.SerializeObject(new MessageEnvelope()
                {
                    MessageName = message.GetType().Name,
                    Message = message
                }));

        }

        public void Reply(IReply m)
        {
        }

        ~ControlMessageBus()
        {
            receiver.Dispose();
            sender.Dispose();
            context.Dispose();
        }
    }
}