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
        private LeaderBus leaderBus;

        public AppendEntriesService(Node node, LeaderBus leaderBus, RequestState requestState)
        {
            this.node = node;

            var appender = new Thread(() =>
                {
                    this.requestState = requestState;
                    this.leaderBus = leaderBus;
                    var state = DomainRegistry.NodLogEntriesService().NodeState();

                    while (true)
                    {
                        var clientRequest = leaderBus.ReceiveCommand();

                        var followerMessage = new AppendEntries()
                            {
                                LeaderId = state.NodeId,
                                Term = state.Term,
                                LogIndex = state.EntryIndex + 1,
                                PrevTerm = state.PrevTerm(),
                                PrevLogIndex = state.PrevLogIngex(),
                                MachineCommands = new List<object> {clientRequest.Command}
                            };

                        node.Send(followerMessage);
                        requestState.MessageSentNow();

                        WaitForMajorityToReply(followerMessage);

                        state.Append(followerMessage.Term, followerMessage.LogIndex, followerMessage.PrevTerm,
                                     followerMessage.PrevLogIndex, followerMessage.MachineCommands);

                        leaderBus.SendToClient();
                    }
                });

            reference = new ServiceReference(appender);
        }


        public void WaitForMajorityToReply(AppendEntries retryMessage)
        {
            var settings = node.GetSettings();
            var nap = 0;
            while (true)
            {
                if (requestState.IsMajority(retryMessage.Term, retryMessage.LogIndex) >= settings.Majority)
                    return;

                Thread.Sleep(settings.FollowerSla / 3);

                nap++;
                if (nap <= 3)
                    leaderBus.Send(retryMessage);
            }
        }
    }
}