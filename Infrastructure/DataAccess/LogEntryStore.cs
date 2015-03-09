using System.Collections.Generic;
using Core.Log;

namespace DataAccess
{
    public class LogEntryStore : ILogEntryStore
    {
        public void Append(LogEntry le)
        {
        }

        public LogEntry LastAppended()
        {
            return new LogEntry() { NodeId = 1, Term = 1, Index = 0, MachineCommands = new List<object>()};
        }
    }
}
