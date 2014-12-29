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
                   var settings = node.GetSettings();
                   var state = DomainRegistry.NodLogEntriesService().NodeState();

                    while (true)
                    {
                        if (DateTime.Now.Subtract(requestState.LastMessageSent()).TotalMilliseconds >= settings.HeartBeatPeriod)
                        {
                            node.Send(new AppendEntries { Term = state.Term, LogIndex = state.EntryIndex, MachineCommands = new List<object>() });
                            Thread.Sleep(settings.HeartBeatPeriod);
                        }
                    }
            });

           reference = new ServiceReference(beat);
        }
    }
}