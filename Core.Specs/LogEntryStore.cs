using System.Collections.Generic;
using Core.Log;

namespace Core.Specs
{
    public class LogEntryStore : ILogEntryStore
    {
        public Stack<LogEntry> stack = new Stack<LogEntry>(); 

        public void Append(LogEntry le)
        {
            stack.Push(le);
        }

        public LogEntry LastAppended()
        {
            return stack.Peek();
        }

        public int Count()
        {
            return stack.Count;
        }
    }
}