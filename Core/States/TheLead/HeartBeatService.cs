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
            var beat = new Thread(() =>
                {
                    var settings = node.GetRegistry().NodeSettings();
                    var state = node.GetRegistry().LogEntriesService().NodeState();

                    while (true)
                    {
                        if (DateTime.Now.Subtract(requestState.LastMessageSent()).TotalMilliseconds >= settings.HeartBeatPeriod)
                        {
                            node.GetRegistry()
                                .NodeMessageSender()
                                .Send(new AppendEntries(state.Term, state.EntryIndex, new List<object>()));
                            
                            Thread.Sleep(settings.HeartBeatPeriod);
                        }
                    }
            });

           reference = new ServiceReference(beat);
        }
    }
}