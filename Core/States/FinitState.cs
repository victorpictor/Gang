using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class FinitState
    {
        protected Node node;

        public virtual void EnterState(ref PersistentNodeState persistentNodeState, Node node)
        {
            this.node = node;
            MessageResponse msgResp;

            while (true)
            {
                var message = NextMessage();

                msgResp = Receive((dynamic)message);

                if (msgResp.LeaveState)
                    break;

                msgResp.NextState(node);
            }

            msgResp.NextState(node);
        }

        public virtual IMessage NextMessage()
        {
            return new TimedOut();
        }

        public void Transition(Action<Node> transition)
        {
           Task.Factory.StartNew(() => transition(node));
        }

        public virtual MessageResponse Receive(AppendEntries appendEntries)
        {
            return new MessageResponse(false, n => { });
        }

        public virtual MessageResponse Receive(RequestedVote requestedVote)
        {
            return new MessageResponse(false, n => { });
        }

        public virtual MessageResponse Receive(VoteGranted voteGranted)
        {
            return new MessageResponse(false, n => { });
        }
    }
}