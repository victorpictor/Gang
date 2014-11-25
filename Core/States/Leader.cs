using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Leader : FinitState, ILead
    {
        protected DateTime lastRequest;

        public override void EnterState(Node node)
        {
            this.lastRequest = DateTime.Now;
            base.node = node;

            base.EnterState(node);

            HeartBeat();
            Appender();
        }


        public void Appender()
        {
            var appender = new Thread(() =>
            {
                var settings = node.GetSettings();
                var state = node.GetState();

                while (true)
                {
                    //get command
                    //send command to followers
                    //wait for majority to respond
                    //retry if is the case
                    //ack the client 
                }
            });

            parallelTasks.Add(appender);
            appender.Start();
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