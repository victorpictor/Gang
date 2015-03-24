using System;
using System.Collections.Generic;
using Core.Clustering;

namespace Core.Log
{
    public class NodeLogEntriesService
    {
        private PersistentNodeState persistentNodeState;
        private ILogEntryStore logEntryStore;
        
        public NodeLogEntriesService(ILogEntryStore logEntryStore, int nodeId)
        {
            this.logEntryStore = logEntryStore;

            var last = logEntryStore.LastAppended();

            this.persistentNodeState = new PersistentNodeState()
            {
                NodeId = nodeId,
                Term = last.Term,
                EntryIndex = last.Index
            };
        }

        public PersistentNodeState NodeState()
        {
            return persistentNodeState;
        }

        public void IncrementTerm()
        {
            persistentNodeState.Term++;
        }

        public void MessageReceivedNow()
        {
           persistentNodeState.LastMessageReceivedOn = DateTime.Now;
        }

        public void UpdateTerm(long term)
        {
            persistentNodeState.Term = term;
        }

        public DateTime LastMessageReceivedOn()
        {
            return persistentNodeState.LastMessageReceivedOn;
        }

        public long PrevTerm()
        {
            return persistentNodeState.PrevTerm();
        }

        public long PrevLogIngex()
        {
            return persistentNodeState.PrevLogIngex();
        }

        public void Append(long term, long currentEntryIndex, long prevTerm, long prevEntryIndex, List<object> messages)
        {
            persistentNodeState.Append(term, currentEntryIndex, prevTerm, PrevLogIngex(), messages, logEntryStore);
        }
    }
}