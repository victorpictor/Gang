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

            //RegisterService(
            //    new ReadLogService()
            //        .Reference());

            StartRegisteredServices();
        }

        public MessageResponse Receive(EntriesAppended entriesAppended)
        {
            leaderBus.Deliver(entriesAppended);

            return new MessageResponse(false, () => { });
        }


    }
}