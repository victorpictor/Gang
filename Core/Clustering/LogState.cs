using System.Collections.Generic;
using Core.Log;

namespace Core.Clustering
{
    public class LogState
    {
        protected long Term;
        protected long EntryIndex;

        protected List<LogEntry> LogEntries;

        protected LogEntry PendingCommit;
    }
}