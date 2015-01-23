using System;
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

        public DateTime LastMessageReceivedOn = DateTime.Now;

        public long PrevTerm()
        {
            if (PendingCommit != null)
                return PendingCommit.Term;

            return Term;
        }

        public long PrevLogIngex()
        {
            if (PendingCommit != null)
                return PendingCommit.Index;

            return EntryIndex;
        }

        public void Append(long term, long currentEntryIndex,long prevTerm, long prevEntryIndex, List<object> messages)
        {
            if (PendingCommit != null)
            if (PendingCommit.Index == prevEntryIndex && PendingCommit.Term == prevTerm)
                LogEntries.Add(PendingCommit);

            Term = term;
            EntryIndex = currentEntryIndex;
            PendingCommit = new LogEntry() { Term = term, Index = currentEntryIndex, MachineCommands = messages };
        }
    }
}