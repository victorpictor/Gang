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
            Action timer = () =>
                {
                    var settings = node.GetRegistry().NodeSettings();
                    
                    var state = node.GetRegistry().LogEntriesService().NodeState();

                    var lastMessageReceivedOn = node.GetRegistry().LogEntriesService().LastMessageReceivedOn();

                    while (DateTime.Now.Subtract(lastMessageReceivedOn).TotalMilliseconds <= settings.ElectionTimeout)
                    {
                    }

                    node.GetRegistry()
                        .DomainMessageSender()
                        .Send(new TimedOut(state.NodeId, state.Term));
                };

            reference = new ServiceReference(timer);
        }
    }
}