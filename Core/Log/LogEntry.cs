using System;
using System.Collections.Generic;

namespace Core.Log
{
    public class LogEntry
    {
        public Guid Id = Guid.NewGuid();

        public long Term;
        public long Index;

        public List<object> MachineCommands;
    }
}