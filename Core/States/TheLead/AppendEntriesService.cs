using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using Core.States.Services;
using Core.Transport;

namespace Core.States.TheLead
{
    public class AppendEntriesService : AbstractService
    {
        private Node node;
        private RequestState requestState;
        
        public AppendEntriesService(Node node, LeaderBus leaderBus, RequestState requestState)
        {
            this.node = node;

            Action appender = () =>
                {
                    this.requestState = requestState;

                    var state = node.GetRegistry().LogEntriesService().NodeState();
                    
                    while (!IsServiceShuttingDown())
                    {
                         this.requestState.FreshState();
                         var clientRequest = leaderBus.ReceiveCommand();

                        var followerMessage = new AppendEntries(state.NodeId, state.Term, state.PrevTerm(),
                                                                state.EntryIndex + 1, state.PrevLogIngex(),
                                                                new List<object> {clientRequest.Command});

                         node.GetRegistry()
                             .NodeMessageSender()
                             .Send(followerMessage);

                         this.Info(string.Format("Sent client command, term {0} index {1}", followerMessage.Term, followerMessage.LogIndex));
                         requestState.MessageSentNow();
                         
                         WaitForMajorityToReply(followerMessage);

                        if (requestState.Failed())
                        {
                            leaderBus.SendToClient(new ClientReply(clientRequest.Id, Status.Failed));
                            continue;
                        }

                        node.GetRegistry()
                            .LogEntriesService()
                            .Append(followerMessage.Term, followerMessage.LogIndex, followerMessage.PrevTerm,
                                    followerMessage.PrevLogIndex, followerMessage.MachineCommands);
                        
                        this.Info(string.Format("Followers received command, term {0} index {1}", followerMessage.Term, followerMessage.LogIndex));

                        leaderBus.SendToClient(new ClientReply(clientRequest.Id, Status.Succeeded));
                    }
                };

            reference = new ServiceReference(appender);
        }


        public void WaitForMajorityToReply(AppendEntries retryMessage)
        {
            var settings = node.GetRegistry().NodeSettings();
            var nap = 0;
            
            while (!IsServiceShuttingDown())
            {
                if (requestState.IsMajority(retryMessage.Term, retryMessage.LogIndex) >= settings.Majority)
                {
                    requestState.Succeeded();
                    return;
                }
                
                Thread.Sleep(settings.FollowerSla / 3);
                
                nap++;
                if (nap <= 3)
                   node.GetRegistry()
                        .NodeMessageSender()
                        .Send(retryMessage);
                else
                    return;
            }
        }
    }
}