﻿namespace Core.Messages
{
    public class TimedOut : IMessage
    {
        public long NodeId { get; set; }
        public long Term { get; set; }
    }
}