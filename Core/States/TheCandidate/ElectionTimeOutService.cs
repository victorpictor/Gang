using System;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;

namespace Core.States.TheCandidate
{
    public class ElectionTimeOutService : AbstractService
    {
        public ElectionTimeOutService(Node node, ElectionState electionState)
        {
            Action timer = () =>
                {
                    var settings = node.GetRegistry().NodeSettings();
                    
                    var state = node.GetRegistry().LogEntriesService().NodeState();

                    Thread.Sleep(settings.ElectionTimeout);

                    var started = DateTime.Now;

                    while (DateTime.Now.Subtract(started).TotalMilliseconds <= settings.ElectionTimeout)
                    {
                    }

                    if (electionState.Votes() <= settings.Majority)
                        node.GetRegistry()
                            .DomainMessageSender()
                            .Send(new TimedOut(state.NodeId, state.Term));
            };

           reference = new ServiceReference(timer);
        } 
    }
}