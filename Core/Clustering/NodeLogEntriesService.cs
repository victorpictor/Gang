using System.Collections.Generic;

namespace Core.Clustering
{
    public class NodeLogEntriesService
    {
        private PersistentNodeState persistentNodeState;

        
        public NodeLogEntriesService(PersistentNodeState persistentNodeState)
        {
            this.persistentNodeState = persistentNodeState;
        }

        public NodeLogEntriesService()
        {

        }

        public void Start()
        {
        } 

        public void Stop()
        {
        }

        public PersistentNodeState NodeState()
        {
            return persistentNodeState;
        }

        public void IncrementTerm()
        {
            persistentNodeState.Term++;
        }

        public void UpdateTerm(long term)
        {
            persistentNodeState.Term = term;
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
            persistentNodeState.Append(term, currentEntryIndex, prevTerm, PrevLogIngex(), messages);
        }
    }
}