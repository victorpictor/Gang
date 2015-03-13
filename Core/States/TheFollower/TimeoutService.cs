using System;
using Core.Clustering;
using Core.Messages.Control;
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
                    
                    node.GetRegistry().LogEntriesService().MessageReceivedNow();
                   
                    var lastMessageReceivedOn = node.GetRegistry().LogEntriesService().LastMessageReceivedOn();

                    while (DateTime.Now.Subtract(lastMessageReceivedOn).TotalMilliseconds <= settings.FollowerTimeout)
                    {
                        lastMessageReceivedOn = node.GetRegistry().LogEntriesService().LastMessageReceivedOn();
                    }

                    Console.WriteLine("{0} Last message received on {1}",DateTime.Now, lastMessageReceivedOn);

                    node.GetRegistry()
                        .ContolMessageSender()
                        .Send(new TimedOut(state.NodeId, state.Term));
                };

            reference = new ServiceReference(timer);
        }
    }
}