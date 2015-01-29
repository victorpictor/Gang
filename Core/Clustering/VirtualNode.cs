using Core.States;

namespace Core.Clustering
{
    public abstract class VirtualNode
    {
        protected NodeState nodeState;
        protected DomainRegistry domainRegistry;
        
        public abstract void Start();

        public abstract void Stop();

        public DomainRegistry GetRegistry()
        {
            return domainRegistry;
        }

        public FinitState LastFinitState()
        {
            return nodeState.ReadState();
        }
        
    }
}