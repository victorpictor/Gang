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
                var settigs = node.GetSettings();
                var state = DomainRegistry.NodLogEntriesService().NodeState();
                var electionStarted = DateTime.Now;

                while (DateTime.Now.Subtract(electionStarted).TotalMilliseconds < settigs.ElectionTimeout)
                {
                    node.Send(new RequestedVote() { CandidateId = state.NodeId, LastLogTerm = state.Term, LastLogIndex = state.EntryIndex });
                    Thread.Sleep(settigs.ElectionTimeout / 3);
                }

            });

            reference = new ServiceReference(election);
        }
    }
}