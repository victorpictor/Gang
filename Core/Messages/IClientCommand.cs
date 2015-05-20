using System;

namespace Core.Messages
{
    public interface IClientCommand : IMessage
    {
        Guid Id { get; set; }
        object Command { get; set; }
    }

    public class ClientCommand : IClientCommand
    {
        public Guid Id { get; set; }
        public object Command { get; set; }
    }
}