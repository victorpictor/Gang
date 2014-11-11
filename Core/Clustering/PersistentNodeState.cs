using System.Collections.Generic;
using Core.Log;

namespace Core.Clustering
{
    public class PersistentNodeState
    {
        public int NodeId;
        public long Term;
        public long EntryIndex;

        public List<LogEntry> LogEntries;

        public LogEntry PendingCommit;
    }
}