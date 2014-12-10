using Core.Clustering;
using Core.Concurrency;
using Core.Transport;

namespace Core.States.TheLead
{
    public class Leader : FinitState
    {
        protected LeaderBus leaderBus;
        protected RequestState requestState;
        protected AppendEntriesReplies appendEntriesReplies;

        public override void EnterState(Node node)
        {

            leaderBus = new LeaderBus();
            this.requestState = new RequestState();

            base.node = node;
           

            RegisterService(
                new AppendEntriesService(base.node, leaderBus, requestState)
                    .Reference());

            RegisterService(
                new HeartBeatService(base.node, this.requestState)
                    .Reference());

            RegisterService(
                new FollowersRepliesService(leaderBus, requestState)
                    .Reference());

            base.EnterState(node);

            StartRegisteredServices();
        }
    }
}