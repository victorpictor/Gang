using Core.Messages;
using Core.States;

namespace Core.Clustering
{
    public class Node: VirtualNode
    {
        public Node(FinitState finitState, DomainRegistry domainRegistry)
        {
            this.nodeState = new NodeState(this, finitState, domainRegistry);

            this.domainRegistry = domainRegistry;
        }

        public override void Start()
        {
            nodeState.EnterState();
        }

        public override void Stop()
        {
            domainRegistry.DomainMessageSender().Send(new ExitState());
        }

        public void Next(FinitState finitState)
        {
            nodeState = new NodeState(this, finitState, domainRegistry);
           
            nodeState.EnterState();
        }

    }
}