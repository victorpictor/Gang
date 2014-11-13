using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class FinitState
    {
        protected Node node;
        protected List<Thread> parallelTasks = new List<Thread>();

        public virtual void EnterState(ref PersistentNodeState persistentNodeState, Node node)
        {
            var loop = new Thread(() =>
                {
                    this.node = node;

                    MessageResponse msgResp;

                    while (true)
                    {
                        var message = NextMessage();

                        msgResp = Receive((dynamic) message);

                        if (msgResp.LeaveState)
                            break;

                        msgResp.Action();
                    }

                    Transition(() => msgResp.Action());
                });

            loop.Start();
        }

        public virtual IMessage NextMessage()
        {
            while (true)
            {
                var message =  node.Receive();

                if (message.Term >= 0)
                    return message;
            }
            
        }

        public void Transition(Action transition)
        {
           Task.Factory.StartNew(transition);
        }

        public virtual MessageResponse Receive(ExitState exitState)
        {
            return new MessageResponse(true, () =>
                {
                    parallelTasks.ForEach(pt => pt.Abort());
                    parallelTasks = new List<Thread>();
                });
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