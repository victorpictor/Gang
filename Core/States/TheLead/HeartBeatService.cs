using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;

namespace Core.States.TheLead
{
    public class HeartBeatService : AbstractService
    {
        public HeartBeatService(Node node, RequestState requestState)
        {
            Action beat = () =>
                {
                    var settings = node.GetRegistry().NodeSettings();
                    var state = node.GetRegistry().LogEntriesService().NodeState();

                    while (true)
                    {
                        if (DateTime.Now.Subtract(requestState.LastMessageSent()).TotalMilliseconds >= settings.HeartBeatPeriod)
                        {
                            node.GetRegistry()
                                .NodeMessageSender()
                                .Send(new AppendEntries(state.Term, state.EntryIndex, new List<object>()){LeaderId = settings.NodeId});
                            Console.WriteLine("Sent heartbeat term {0}", state.Term);
                            Thread.Sleep(settings.HeartBeatPeriod - 1);
                        }
                    }
            };

           reference = new ServiceReference(beat);
        }
    }
}