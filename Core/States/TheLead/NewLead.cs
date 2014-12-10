using Core.Clustering;
using Core.Concurrency;
using Core.Transport;

namespace Core.States.TheLead
{
    public class NewLead : FinitState
    {
        private LeaderBus leaderBus;
        private RequestState requestState;
        protected AppendEntriesReplies appendEntriesReplies;

        public override void EnterState(Node node)
        {

            leaderBus = new LeaderBus();
            this.requestState = new RequestState();

            base.node = node;
            base.EnterState(node);


            RegisterService(
                new AppendEntriesService(base.node, leaderBus, requestState)
                    .StartService().Reference());

            RegisterService(
                new HeartBeatService(base.node, this.requestState)
                    .StartService().Reference());

            RegisterService(
                new FollowersRepliesService(leaderBus, requestState)
                    .StartService().Reference());

            StartRegisteredServices();
        }
    }
}