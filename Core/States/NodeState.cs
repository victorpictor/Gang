using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.Receivers;

namespace Core.States
{
    public class NodeState
    {
        private IReceiveMessages receiver;

        public virtual void EnterState(Node node, FinitState state, IReceiveMessages receiver)
        {
            this.receiver = receiver;

            state.EnterNewState(node);

            var loop = new Thread(() =>
            {
                MessageResponse msgResp;

                while (true)
                {
                    var message = NextMessage();

                    msgResp = state.Receive((dynamic)message);

                    if (msgResp.LeaveState)
                        break;

                    msgResp.Action();
                }

                state.Transition(() => msgResp.Action());
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