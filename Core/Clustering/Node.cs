using Core.Messages;
using Core.States;

namespace Core.Clustering
{
    public class Node
    {
        private NodeState nodeState;
        
        private DomainRegistry domainRegistry;
        
        public Node(FinitState finitState, DomainRegistry domainRegistry)
        {
            this.nodeState = new NodeState(this, finitState, domainRegistry);

            this.domainRegistry = domainRegistry;
        }

        public void Start()
        {
            nodeState.EnterState();
        }

        public void Stop()
        {
            domainRegistry.DomainMessageSender().Send(new ExitState());
        }

        
        public void Next(FinitState finitState)
        {
            nodeState = new NodeState(this, finitState, domainRegistry);
           
            nodeState.EnterState();
        }
       
        public FinitState LastFinitState()
        {
            return nodeState.ReadState();
        }
        
        public DomainRegistry GetRegistry()
        {
            return domainRegistry;
        }

    }
}