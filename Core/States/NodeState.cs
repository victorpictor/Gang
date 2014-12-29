using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.Receivers;

namespace Core.States
{
    public class NodeState
    {
        private Node node;
        private IReceiveMessages receiver;
        private FinitState finitState;

        public NodeState(Node node, FinitState state, IReceiveMessages receiver)
        {
            this.node = node;
            this.receiver = receiver;
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
            while (true)
            {
                var message = receiver.Receive();

                if (message.Term >= 0)
                    return message;
            }

        }
    }
}