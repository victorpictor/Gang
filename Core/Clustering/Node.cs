using Core.Messages;
using Core.States;

namespace Core.Clustering
{
    public class Node: VirtualNode, IChangeState
    {
        public Node(FinitState finitState, DomainRegistry domainRegistry)
        {
            base.nodeState = new NodeState(this, finitState, domainRegistry);

            base.domainRegistry = domainRegistry;
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