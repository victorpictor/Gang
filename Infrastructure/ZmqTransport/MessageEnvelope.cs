using System;
using Core.Messages;

namespace ZmqTransport
{
    public class MessageEnvelope
    {
        public Guid Id = Guid.NewGuid();

        public int SenderId;
        public string MessageName;
        public string Message;
    }
}