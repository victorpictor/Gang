using System;
using Core.Messages;

namespace ZmqTransport
{
    public class MessageEnvelope
    {
        public Guid Id = Guid.NewGuid();

        public string MessageName;
        public dynamic Message;
    }
}