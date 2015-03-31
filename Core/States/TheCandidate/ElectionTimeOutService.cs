using System;
using System.Threading;
using Core.Clustering;
using Core.Messages.Control;
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

                    var electionEnds = DateTime.Now.AddMilliseconds(settings.ElectionTimeout);

                    while (DateTime.Now < electionEnds && !IsServiceShuttingDown())
                    {
                        if (electionState.Votes() >= settings.Majority)
                            break;

                        Thread.Sleep(50);
                    }

                    if (electionState.Votes() <= settings.Majority && !IsServiceShuttingDown())
                        node.GetRegistry()
                            .ContolMessageSender()
                            .Send(new TimedOut(state.NodeId, state.Term));
            };

           reference = new ServiceReference(timer);
        } 
    }
}