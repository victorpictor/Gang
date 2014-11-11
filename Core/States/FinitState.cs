using System;
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

                msgResp.Action();
            }

            Transition(() => msgResp.Action());
        }

        public virtual IMessage NextMessage()
        {
            return new TimedOut();
        }

        public void Transition(Action transition)
        {
           Task.Factory.StartNew(transition);
        }

        public virtual MessageResponse Receive(AppendEntries appendEntries)
        {
            return new MessageResponse(false, () => { });
        }

        public virtual MessageResponse Receive(RequestedVote requestedVote)
        {
            return new MessageResponse(false, () => { });
        }

        public virtual MessageResponse Receive(VoteGranted voteGranted)
        {
            return new MessageResponse(false, () => { });
        }

        public virtual MessageResponse Receive(TimedOut timedOut)
        {
            return new MessageResponse(true, () => { });
        }
    }
}