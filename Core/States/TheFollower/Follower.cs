using System;
using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;
using Core.Messages.Control;

namespace Core.States.TheFollower
{
    public class Follower : FinitState
    {
        protected Dictionary<long, int> votes = new Dictionary<long, int>();

        public Follower(){}
        public Follower(Dictionary<long, int> votes)
        {
            this.votes = votes;
        }


        public override void EnterNewState(Node node)
        {
            Console.WriteLine("Node is {0}", this.GetType().Name);

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
            Console.WriteLine("{3} Received AppendEntries from node {0}, term {1} log index {2}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex, DateTime.Now);
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

            Console.WriteLine("{2} Requested vote, candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term, DateTime.Now);

            if (state.Term < requestedVote.LastLogTerm)
                return new MessageResponse(false, () => { });

            if (state.Term < requestedVote.Term)
            {
                if (!votes.ContainsKey(requestedVote.Term))
                {
                    votes.Add(requestedVote.Term, requestedVote.CandidateId);

                    Console.WriteLine("{2} Vote granted for {0}, term {1}", requestedVote.CandidateId, requestedVote.Term, DateTime.Now);
                    node
                        .GetRegistry()
                        .LogEntriesService().MessageReceivedNow();

                    node.GetRegistry()
                        .NodeMessageSender()
                        .Send(new VoteGranted(state.NodeId, requestedVote.CandidateId, requestedVote.Term));

                }
                else
                {
                    Console.WriteLine("{3}Already voted in this term for candidate {0}, term {1}, my term was {2}",
                                      requestedVote.CandidateId, requestedVote.Term, state.Term, DateTime.Now);
                }
            }
            else
            {
                Console.WriteLine("{2} Did not grant vote for candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term, DateTime.Now);
            }

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(TimedOut timedOut)
        {
            Console.WriteLine("{0} Timed out", DateTime.Now);
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