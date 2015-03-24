﻿using System;
using System.Collections.Generic;
using Core.Clustering;
using Core.Messages;
using Core.Messages.Control;

namespace Core.States.TheCandidate
{
    public class Candidate : FinitState
    {
        private ElectionState Granted = new ElectionState();
        protected Dictionary<long, int> votes = new Dictionary<long, int>();

        public override void EnterNewState(Node node)
        {
            Console.WriteLine("Node is {0}", this.GetType().Name);

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
            Console.WriteLine("{3} Received AppendEntries from node {0}, term {1} log index {2}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex, DateTime.Now);

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
            
            Console.WriteLine("{3} Ignored AppendEntries from node {0}, term {1} log index {2} my term was {4} index {5}", appendEntries.LeaderId, appendEntries.Term, appendEntries.LogIndex, DateTime.Now, state.Term, state.EntryIndex);

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(VoteGranted voteGranted)
        {
            Console.WriteLine("{2} Voted by {0}, term {1}", voteGranted.VoterId, voteGranted.Term, DateTime.Now);
            
            var settings = node.GetRegistry().NodeSettings();
            
            if (voteGranted.CandidateId == settings.NodeId)
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
            Console.WriteLine("{2} Requested vote, candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term, DateTime.Now);

            var state = node.GetRegistry().LogEntriesService().NodeState();
            
            if (state.Term < requestedVote.Term)
            {
                if (!votes.ContainsKey(requestedVote.Term))
                {
                    votes.Add(requestedVote.Term, requestedVote.CandidateId);
                    
                    Console.WriteLine("{3} Voted for candidate {0}, term {1}, my term was {2}", requestedVote.CandidateId, requestedVote.Term, state.Term, DateTime.Now);
                    
                    node.GetRegistry()
                      .NodeMessageSender()
                      .Send(new VoteGranted(state.NodeId, requestedVote.CandidateId, requestedVote.LastLogTerm));

                    return new MessageResponse(true, () =>
                        {
                            StopRegisteredServices();
                            node.Next(new StateFactory().Follower(votes));
                        });
                }
                
                Console.WriteLine("{3} Already voted in this term for candidate {0}, term {1}, my term was {2}", requestedVote.CandidateId, requestedVote.Term, state.Term,DateTime.Now);
            }
            else
                Console.WriteLine("{2} Did not grant vote for candidate {0}, term {1}", requestedVote.CandidateId, requestedVote.Term, DateTime.Now);
            

            return new MessageResponse(false, () => { });
        }


        public override MessageResponse Receive(TimedOut timedOut)
        {
            Console.WriteLine("{0} Election timed out", DateTime.Now);
            return new MessageResponse(true, () =>
            {
                node.GetRegistry().LogEntriesService().IncrementTerm();
                StopRegisteredServices();
                node.Next(new StateFactory().Candidate());
            });
        }
    }
}