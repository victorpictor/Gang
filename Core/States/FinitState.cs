using System;
using System.Threading.Tasks;
using Core.Clustering;
using Core.Messages;
using Core.Messages.Control;

namespace Core.States
{
    public class FinitState: StateServiceCollection
    {
        protected Node node;
       
        public virtual void EnterNewState(Node node) { }
        
        public void Transition(Action transition)
        {
           Task.Factory.StartNew(transition);
        }
        
        public virtual MessageResponse Receive(ExitState exitState)
        {
            return new MessageResponse(true, StopRegisteredServices);
        }

        public virtual MessageResponse Receive(EntriesAppended entriesAppended)
        {
            return new MessageResponse(false, () => { });
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