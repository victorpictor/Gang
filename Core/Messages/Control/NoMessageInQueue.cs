using System;

namespace Core.Messages.Control
{
    public class NoMessageInQueue:IMessage, IClientCommand
    {
        public NoMessageInQueue()
        {
            Term = -1;
        }

        public Guid Id { get; set; }
        public long Term { get; set; }
        public object Command { get; set; }
    }
}