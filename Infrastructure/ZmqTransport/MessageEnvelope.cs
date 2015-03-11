using System;
using Core.Messages;

namespace ZmqTransport
{
    public class MessageEnvelope
    {
        public string topic;

        public MessageEnvelope(string topic)
        {
            this.topic = topic;
        }

        public Guid Id = Guid.NewGuid();

        public int SenderId;
        public string MessageName;
        public dynamic Message;
    }
}