using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;

namespace Core.States.TheFollower
{
    public class Follower : FinitState
    {
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
            var logEntryService = node
                .GetRegistry()
                .LogEntriesService();

            var state = logEntryService.NodeState();

            if (appendEntries.Term < state.Term)
                return new MessageResponse(false, () => { });

            if (appendEntries.Term > state.Term)
                logEntryService.UpdateTerm(appendEntries.Term);

            logEntryService.MessageReceivedNow();

            if (!appendEntries.IsHeartBeat())
            {
                logEntryService
                    .Append(appendEntries.Term,
                            appendEntries.LogIndex,
                            appendEntries.PrevTerm,
                            appendEntries.PrevLogIndex,
                            appendEntries.MachineCommands);

                node
                    .GetRegistry()
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
                    votes.Add(requestedVote.LastLogTerm, requestedVote.CandidateId);

                    node.GetRegistry()
                        .NodeMessageSender()
                        .Send(new VoteGranted(state.NodeId, requestedVote.CandidateId, requestedVote.LastLogTerm));
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