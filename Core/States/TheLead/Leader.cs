using System;
using Core.Clustering;
using Core.Messages;
using Core.Transport;

namespace Core.States.TheLead
{
    public class Leader : FinitState
    {
        protected LeaderBus leaderBus;
        protected RequestState requestState;

        public override void EnterNewState(Node node)
        {

            this.Info(string.Format("Node is {0}", this.GetType().Name));

            base.node = node;

            leaderBus = new LeaderBus();
            this.requestState = new RequestState();

            RegisterService(
                new AppendEntriesService(base.node, leaderBus, requestState)
                    .Reference());

            RegisterService(
                new HeartBeatService(base.node, requestState)
                    .Reference());

            RegisterService(
                new FollowersRepliesService(leaderBus, requestState)
                    .Reference());

            RegisterService(
                new ReadLogService()
                    .Reference());

            StartRegisteredServices();
        }

        public override MessageResponse Receive(EntriesAppended entriesAppended)
        {
            leaderBus.Deliver(entriesAppended);
            this.Info(string.Format("EntriesAppended by {0} term {1} index {2}", entriesAppended.NodeId, entriesAppended.Term, entriesAppended.LogIndex));
            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(RequestedVote requestedVote)
        {
            this.Info(string.Format("Requested vote, candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term));

            var state = node.GetRegistry().LogEntriesService().NodeState();

            if (state.Term < requestedVote.Term)
            {
                this.Info(string.Format("Left leader state"));
                return new MessageResponse(true, () =>
                {
                    StopRegisteredServices();
                    node.Next(new StateFactory().Follower());
                });
            }

            return new MessageResponse(false, () => { });
        }
    }
}