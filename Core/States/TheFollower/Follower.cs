using System;
using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;

namespace Core.States.TheFollower
{
    public class Follower : FinitState
    {
        protected DateTime lastReceivedOn = DateTime.Now;
        protected Dictionary<long, int> votes = new Dictionary<long, int>();
        
        public override void EnterNewState(Node node)
        {
            base.node = node;

            RegisterService(
                new TimeoutService(node)
                    .Reference());

            RegisterService(
                new LogRecoveryService()
                    .Reference());

            StartRegisteredServices();
        }
        
        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            var state = node
                .GetRegistry()
                .LogEntriesService()
                .NodeState();

            if (appendEntries.Term < state.Term)
                return new MessageResponse(false, () => { });

            if (appendEntries.Term > state.Term)
                node
                    .GetRegistry()
                    .LogEntriesService()
                    .UpdateTerm(appendEntries.Term);

            lastReceivedOn = DateTime.Now;

            if (!appendEntries.IsHeartBeat())
            {
                node
                    .GetRegistry()
                    .LogEntriesService()
                    .Append(appendEntries.Term, appendEntries.LogIndex, appendEntries.PrevTerm,appendEntries.PrevLogIndex, appendEntries.MachineCommands);

                node.GetRegistry()
                    .NodeMessageSender()
                    .Send(new EntriesAppended()
                        {
                            Term = appendEntries.Term,
                            LogIndex = appendEntries.LogIndex,
                            NodeId = node.GetRegistry().NodeSettings().NodeId
                        });
            }

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(RequestedVote requestedVote)
        {
            var state = node.GetRegistry().LogEntriesService().NodeState();

            if (state.Term < requestedVote.LastLogTerm)
                return new MessageResponse(false, () => { });

            if (state.Term == requestedVote.LastLogTerm)
            if (!votes.ContainsKey(requestedVote.LastLogTerm))
            {
                votes.Add(requestedVote.LastLogTerm,requestedVote.CandidateId);

                node.GetRegistry()
                    .NodeMessageSender()
                    .Send(new VoteGranted()
                        {
                            VoterId = state.NodeId,
                            CandidateId = requestedVote.CandidateId,
                            Term = requestedVote.LastLogTerm
                        });
            }

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(TimedOut timedOut)
        {
            return new MessageResponse(true, () =>
                {
                    node
                        .GetRegistry()
                        .LogEntriesService()
                        .IncrementTerm();

                    node.Next(new StateFactory().Candidate());
                });
        }

        public override MessageResponse Receive(ExitState exitState)
        {
            return new MessageResponse(true, StopRegisteredServices);
        }

    }
}