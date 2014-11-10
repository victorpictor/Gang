using System;

namespace Core.Log
{
    public class LogEntry
    {
        public Guid Id = Guid.NewGuid();

        public long Term;
        public long Index;

        public object Message;
    }
}