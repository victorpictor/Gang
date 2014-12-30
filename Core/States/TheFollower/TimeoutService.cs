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
                    var settings = node.GetSettings();
                    var state = node
                        .NodLogEntriesService()
                        .NodeState();
                    var started = DateTime.Now;

                    while (DateTime.Now.Subtract(started).TotalMilliseconds <= settings.ElectionTimeout)
                    {
                    }

                    node.Send(new TimedOut() {NodeId = state.NodeId, Term = state.Term});
                });

            reference = new ServiceReference(timer);
        }
    }
}