using System;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;

namespace Core.States.TheCandidate
{
    public class ElectionService : AbstractService
    {
        public ElectionService(Node node)
        {
            var election = new Thread(() =>
            {
                var settigs = node.GetRegistry().NodeSettings();
                
                var state = node.GetRegistry().LogEntriesService().NodeState();
                
                var electionStarted = DateTime.Now;

                while (DateTime.Now.Subtract(electionStarted).TotalMilliseconds < settigs.ElectionTimeout)
                {
                    node.GetRegistry()
                        .NodeMessageSender()
                        .Send(new RequestedVote(state.NodeId, state.PrevTerm(), state.Term, state.EntryIndex));
                    
                    Thread.Sleep(settigs.ElectionTimeout / 3);
                }

            });

            reference = new ServiceReference(election);
        }
    }
}