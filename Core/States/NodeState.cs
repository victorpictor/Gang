using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.Receivers;

namespace Core.States
{
    public class NodeState
    {
        private Node node;
        private DomainRegistry registry;
        private FinitState finitState;

        public NodeState(Node node, FinitState state, DomainRegistry registry)
        {
            this.node = node;
            this.registry = registry;
            this.finitState = state;
        }

        public FinitState ReadState()
        {
            return finitState;
        }

        public virtual void EnterState()
        {
            
            finitState.EnterNewState(node);

            var loop = new Thread(() =>
            {
                MessageResponse msgResp;

                while (true)
                {
                    var message = NextMessage();

                    msgResp = finitState.Receive((dynamic)message);

                    if (msgResp.LeaveState)
                        break;

                    msgResp.Action();
                }

                finitState.Transition(() => msgResp.Action());
            });

            loop.Start();
        }

        public IMessage NextMessage()
        {
            var receiver = registry.MessageReceiver();

            while (true)
            {
                var message = receiver.Receive();

                if (message.Term >= 0)
                    return message;
            }

        }
    }
}