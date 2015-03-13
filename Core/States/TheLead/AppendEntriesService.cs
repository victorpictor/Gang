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

                    while (true)
                    {
                        var clientRequest = leaderBus.ReceiveCommand();

                        var followerMessage = new AppendEntries(state.NodeId, state.Term, state.PrevTerm(),
                                                                state.EntryIndex + 1, state.PrevLogIngex(),
                                                                new List<object> {clientRequest.Command});

                        node.GetRegistry()
                            .NodeMessageSender()
                            .Send(followerMessage);

                        requestState.MessageSentNow();

                        WaitForMajorityToReply(followerMessage);

                        if (requestState.Failed())
                            // send failure to client and continue

                        node.GetRegistry()
                            .LogEntriesService()
                            .Append(followerMessage.Term, followerMessage.LogIndex, followerMessage.PrevTerm,
                                    followerMessage.PrevLogIndex, followerMessage.MachineCommands);

                        leaderBus.SendToClient();
                    }
                };

            reference = new ServiceReference(appender);
        }


        public void WaitForMajorityToReply(AppendEntries retryMessage)
        {
            var settings = node.GetRegistry().NodeSettings();
            var nap = 0;
            
            while (true)
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