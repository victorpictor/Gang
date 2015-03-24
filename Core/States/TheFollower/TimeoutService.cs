using System;
using System.Threading;
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

                    var msAgo = 0;
                    while (msAgo <= settings.FollowerTimeout)
                    {
                        Thread.Sleep(settings.FollowerTimeout - 100);

                        var lastMessageReceivedOn = node.GetRegistry().LogEntriesService().LastMessageReceivedOn();
                        msAgo = (int)DateTime.Now.Subtract(lastMessageReceivedOn).TotalMilliseconds;
                    }

                    Console.WriteLine("{0} Sending time out",DateTime.Now);

                    node.GetRegistry()
                        .ContolMessageSender()
                        .Send(new TimedOut(state.NodeId, state.Term));
                };

            reference = new ServiceReference(timer);
        }
    }
}