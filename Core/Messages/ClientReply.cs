using System;

namespace Core.Messages
{
    public enum Status
    {
        Failed,
        Succeeded
    }

    public class ClientReply
    {
        public ClientReply()
        {
        }

        public ClientReply(Guid commandId, Status status)
        {
            CommandId = commandId;
            Status = status;
        }

        public Guid CommandId { get; set; }
        public Status Status { get; set; }
    }
}