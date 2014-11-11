using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Candidate : FinitState
    {
        private HashSet<int> Granted = new HashSet<int>();

        public override void EnterState(ref PersistentNodeState persistentNodeState, Node node)
        {
            base.node = node;

            Timer();
            RequestVotes();

            EnterState(ref persistentNodeState, node);
        }

        public override MessageResponse Receive(AppendEntries appendEntries)
        {
            var state = node.GetState();
            
            if (appendEntries.Term > state.Term)
                return new MessageResponse(true, () => node.Next(new Follower()));

          return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(VoteGranted voteGranted)
        {
            var settings = node.GetSettings();

            Granted.Add(voteGranted.VoterId);

            if (Granted.Count >= settings.Majority)
                return new MessageResponse(true, () => node.Next(new Leader()));

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(TimedOut timedOut)
        {
            return new MessageResponse(true, () =>
            {
                var state = node.GetState();
                state.Term++;
                node.Next(new Candidate());
            });
        }

        public void RequestVotes()
        {
            var settigs = node.GetSettings();
            var state = node.GetState();
            var electionStarted = DateTime.Now;

            Task.Factory.StartNew(() =>
            {
                while (DateTime.Now.Subtract(electionStarted).TotalMilliseconds < settigs.ElectionTimeout)
                {
                    node.Send(new RequestedVote(){CandidateId = state.NodeId, LastLogTerm = state.Term, LastLogIndex = state.EntryIndex});
                    Thread.Sleep(settigs.ElectionTimeout / 3);
                }
                
            });
        }

        private void Timer()
        {
            var settings = node.GetSettings();
            var state = node.GetState();
            
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(settings.ElectionTimeout);
                
                if (Granted.Count <= settings.Majority)
                    node.Send(new TimedOut() { NodeId = state.NodeId, CurrentTerm = state.Term });
            });
        }
    }
}