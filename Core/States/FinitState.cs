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
            this.Info(string.Format("Received AppendEntries from node {0}, term {1} log index {2}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex));
            var state = node.GetRegistry().LogEntriesService().NodeState();

            if (appendEntries.Term > state.Term)
            {
                node.GetRegistry().LogEntriesService().UpdateTerm(appendEntries.Term);

                return new MessageResponse(true, () =>
                {
                    StopRegisteredServices();
                    node.Next(new StateFactory().Follower());
                });
            }

            this.Info(string.Format("Ignored AppendEntries from node {0}, term {1} log index {2} my term was {3} index {4}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex, state.Term, state.EntryIndex));

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