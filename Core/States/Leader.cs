using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Leader : FinitState, ILead
    {
        private DateTime lastRequest;

        public override void EnterState(Node node)
        {
            this.lastRequest = DateTime.Now;
            this.node = node;

            base.EnterState(node);

            HeartBeat();
        }

        public void HeartBeat()
        {
            var beat = new Thread(() =>
            {
                var settings = node.GetSettings();
                var state = node.GetState();
                
                while (true)
                {
                    if (DateTime.Now.Subtract(lastRequest).TotalMilliseconds >= settings.HeartBeatPeriod)
                    {
                        node.Send(new AppendEntries() {Term = state.Term, LogIndex = state.EntryIndex, MachineCommands = new List<object>()});
                        Thread.Sleep(settings.HeartBeatPeriod);
                    }
                }
            });

            parallelTasks.Add(beat);
            beat.Start();
        }

        public void SyncLog()
        {
            
        }
    }
}