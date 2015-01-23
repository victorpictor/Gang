using System;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;

namespace Core.States.TheFollower
{
    public class TimeoutService : AbstractService
    {
        public TimeoutService(Node node)
        {
            var timer = new Thread(() =>
                {
                    var settings = node.GetRegistry().NodeSettings();
                    
                    var state = node.GetRegistry().LogEntriesService().NodeState();
                   
                    var started = DateTime.Now;

                    while (DateTime.Now.Subtract(started).TotalMilliseconds <= settings.ElectionTimeout)
                    {
                    }

                    node.GetRegistry()
                        .DomainMessageSender()
                        .Send(new TimedOut(state.NodeId, state.Term));
                });

            reference = new ServiceReference(timer);
        }
    }
}