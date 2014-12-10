using System.Threading;
using Core.Messages;
using Core.States.Services;
using Core.Transport;

namespace Core.States.TheLead
{
    public class FollowersRepliesService : AbstractService
    {
        public FollowersRepliesService(LeaderBus leaderBus, RequestState requestState)
        {
            var appended = new Thread(() =>
                {
                    while (true)
                    {
                        var entriesAppended = (EntriesAppended)leaderBus.ReceiveMessage();
                        requestState.RegisterReply(entriesAppended);
                    }
            });

            reference = new ServiceReference(appended);
        }
    }
}