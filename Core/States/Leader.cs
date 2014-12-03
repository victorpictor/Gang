using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Clustering;
using Core.Concurrency;
using Core.Messages;
using Core.Transport;

namespace Core.States
{
    
    public class Leader : FinitState, ILead
    {
        private LeaderBus leaderBus;

        protected DateTime lastRequest;
        protected AappendEntriesReplies appendEntriesReplies;
        
        public override void EnterState(Node node)
        {
            leaderBus = new LeaderBus();
            this.lastRequest = DateTime.Now;
            
            base.node = node;
            base.EnterState(node);
            

            HeartBeat();
            Appender();
            AppendedEntries();
        }

        public void Appender()
        {
            var appender = new Thread(() =>
            {
                var state = node.GetState();

                while (true)
                {
                    var clientRequest = leaderBus.ReceiveCommand();

                    appendEntriesReplies = new AappendEntriesReplies();

                    var followerMessage = new AppendEntries()
                        {
                            LeaderId = state.NodeId,
                            Term = state.Term,
                            LogIndex = state.EntryIndex +1,
                            PrevTerm = state.PrevTerm(),
                            PrevLogIndex = state.PrevLogIngex(),
                            MachineCommands = new List<object> {clientRequest.Command}
                        };

                    node.Send(followerMessage);
                    lastRequest = DateTime.Now;

                    WaitForMajorityToReply(followerMessage);

                    state.Append(followerMessage.Term, followerMessage.LogIndex, followerMessage.PrevTerm, followerMessage.PrevLogIndex, followerMessage.MachineCommands);

                    leaderBus.SendToClient();
                }
            });

            parallelTasks.Add(appender);
            appender.Start();
        }


        public void WaitForMajorityToReply(AppendEntries retryMessage)
        {
            var settings = node.GetSettings();
            var nap = 0;
            while (true)
            {
                if (appendEntriesReplies.Count(retryMessage.Term, retryMessage.LogIndex) >= settings.Majority)
                    return;
                
                Thread.Sleep(settings.FollowerSla / 3);
                
                nap++;
                if (nap <= 3)
                    leaderBus.Send(retryMessage);
            }
        }

        public void AppendedEntries()
        {
            var appended = new Thread(() =>
            {
               while (true)
                {
                    var entriesAppended = (EntriesAppended)leaderBus.ReceiveMessage();
                    appendEntriesReplies.Register(entriesAppended);
                }
            });

            parallelTasks.Add(appended);
            appended.Start();
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
                        node.Send(new AppendEntries { Term = state.Term, LogIndex = state.EntryIndex, MachineCommands = new List<object>() });
                        Thread.Sleep(settings.HeartBeatPeriod);
                    }
                }
            });

            parallelTasks.Add(beat);
            beat.Start();
        }
    }
}