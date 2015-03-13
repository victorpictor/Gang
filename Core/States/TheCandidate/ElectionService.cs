using System;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;

namespace Core.States.TheCandidate
{
    public class ElectionService : AbstractService
    {
        public ElectionService(Node node)
        {
            Action election = () =>
            {
                var settigs = node.GetRegistry().NodeSettings();

                var state = node.GetRegistry().LogEntriesService().NodeState();

                Console.WriteLine("Node is {0} triggering election, term {1}", settigs.NodeId, state.Term);
                node.GetRegistry()
                    .NodeMessageSender()
                    .Send(new RequestedVote(state.NodeId, state.PrevTerm(), state.Term, state.EntryIndex));

            };

            reference = new ServiceReference(election);
        }
    }
}