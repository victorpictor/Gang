using System;
using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;
using Core.Messages.Control;

namespace Core.States.TheCandidate
{
    public class Candidate : FinitState
    {
        private ElectionState Granted = new ElectionState();

        public override void EnterNewState(Node node)
        {
            this.Info(string.Format("Node is {0}", this.GetType().Name));

            base.node = node;

            RegisterService(
                new ElectionService(base.node)
                    .Reference());

            RegisterService(
                new ElectionTimeOutService(base.node, this.Granted)
                    .Reference());

            StartRegisteredServices();
        }

        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            this.Info(string.Format("Received AppendEntries from node {0}, term {1} log index {2}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex));
            var state = node.GetRegistry().LogEntriesService().NodeState();

            if (appendEntries.Term >= state.Term)
            {
                node.GetRegistry().LogEntriesService().UpdateTerm(appendEntries.Term);

                return new MessageResponse(true, () =>
                    {
                        StopRegisteredServices();
                        node.Next(new StateFactory().Follower());
                    });
            }

            this.Info(string.Format("Ignored AppendEntries from node {0}, term {1} log index {2} my term was {3} index {4}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex, state.Term, state.EntryIndex));

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(VoteGranted voteGranted)
        {
            this.Info(string.Format("Voted by {0}, term {1}", voteGranted.VoterId, voteGranted.Term));

            var settings = node.GetRegistry().NodeSettings();

            if (voteGranted.To == settings.NodeId)
            {
                Granted.AddVote(voteGranted.VoterId);

                if (Granted.Votes() >= settings.Majority)
                    return new MessageResponse(true, () =>
                        {
                            StopRegisteredServices();
                            node.Next(new StateFactory().Leader());
                        });
            }

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(RequestedVote requestedVote)
        {
            this.Info(string.Format("Requested vote, candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term));

            var state = node.GetRegistry().LogEntriesService().NodeState();

            if (state.Term < requestedVote.Term)
            {

                this.Info(string.Format("Voted for candidate {0}, term {1}, my term was {2}", requestedVote.CandidateId, requestedVote.Term, state.Term));

                node.GetRegistry()
                  .NodeMessageSender()
                  .Reply(new VoteGranted(state.NodeId, requestedVote.CandidateId, requestedVote.LastLogTerm));

                return new MessageResponse(true, () =>
                    {
                        StopRegisteredServices();
                        node.Next(new StateFactory().Follower(new Dictionary<long, int>() { { requestedVote.Term, requestedVote.CandidateId } }));
                    });
            }
            else
                this.Info(string.Format("Did not grant vote for candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term));

            return new MessageResponse(false, () => { });
        }


        public override MessageResponse Receive(TimedOut timedOut)
        {
            this.Info("Election timed out");

            return new MessageResponse(true, () =>
            {
                node.GetRegistry().LogEntriesService().IncrementTerm();
                StopRegisteredServices();
                node.Next(new StateFactory().Candidate());
            });
        }
    }
}