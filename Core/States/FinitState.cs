using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;

namespace Core.States
{
    public class FinitState
    {
        protected Node node;
        protected List<ServiceReference> registeredServices = new List<ServiceReference>();

        public virtual void EnterNewState(Node node) { }
        
        public void Transition(Action transition)
        {
           Task.Factory.StartNew(transition);
        }
        
        public virtual void StartRegisteredServices()
        {
            registeredServices.ForEach(service => service.StartService());
        }

        public virtual void StopRegisteredServices()
        {
            registeredServices.ForEach(service => service.StopService());
            registeredServices = new List<ServiceReference>();
        }

        public virtual void RegisterService(ServiceReference service)
        {
            registeredServices.Add(service);
        }
        
        public virtual MessageResponse Receive(ExitState exitState)
        {
            return new MessageResponse(true, StopRegisteredServices);
        }

        public virtual MessageResponse Receive(AppendEntries appendEntries)
        {
            var state = node.GetRegistry().UseLogEntriesService().NodeState();

            if (appendEntries.Term > state.Term)
            {
                node.GetRegistry().UseLogEntriesService().UpdateTerm(appendEntries.Term);

                return new MessageResponse(true, () => node.Next(new StateFactory().Follower()));
            }

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