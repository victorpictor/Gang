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
            var timer = new Thread(() =>
            {
                var settings = node.GetSettings();
                var state = node.GetState();

                Thread.Sleep(settings.ElectionTimeout);

                var started = DateTime.Now;

                while (DateTime.Now.Subtract(started).TotalMilliseconds <= settings.ElectionTimeout)
                {
                }

                if (electionState.Votes() <= settings.Majority)
                    node.Send(new TimedOut() { NodeId = state.NodeId, Term = state.Term });
            });

           reference = new ServiceReference(timer);
        } 
    }
}