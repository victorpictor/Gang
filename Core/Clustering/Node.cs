using Core.Messages;
using Core.Receivers;
using Core.States;

namespace Core.Clustering
{
    public class Node
    {
        private NodeState nodeState;
        
        private DomainRegistry domainRegistry;
        private IReceiveMessages receiver;

        public Node(FinitState finitState, DomainRegistry domainRegistry, IReceiveMessages receiver)
        {
            this.nodeState = new NodeState(this, finitState, receiver);

            this.domainRegistry = domainRegistry;
            this.receiver = receiver;
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
            nodeState = new NodeState(this, finitState, receiver);
           
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