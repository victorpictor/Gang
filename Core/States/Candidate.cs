using System;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Candidate : FinitState
    {
        private HashSet<int> Granted = new HashSet<int>();

        public override void EnterState(Node node)
        {
            base.node = node;

            Timer();
            RequestVotes();

            base.EnterState(node);
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
            var electionTask = new Thread(() =>
            {
                var settigs = node.GetSettings();
                var state = node.GetState();
                var electionStarted = DateTime.Now;

                while (DateTime.Now.Subtract(electionStarted).TotalMilliseconds < settigs.ElectionTimeout)
                {
                    node.Send(new RequestedVote(){CandidateId = state.NodeId, LastLogTerm = state.Term, LastLogIndex = state.EntryIndex});
                    Thread.Sleep(settigs.ElectionTimeout / 3);
                }
                
            });

            parallelTasks.Add(electionTask);
            electionTask.Start();
        }

        private void Timer()
        {
            var timer = new Thread(() =>
            {
                var settings = node.GetSettings();
                var state = node.GetState();

                Thread.Sleep(settings.ElectionTimeout);

                var started = DateTime.Now;

                while (DateTime.Now.Subtract(started).TotalMilliseconds <= settings.ElectionTimeout)
                {
                }

                if (Granted.Count <= settings.Majority)
                    node.Send(new TimedOut() { NodeId = state.NodeId, Term = state.Term });
            });

            parallelTasks.Add(timer);
            timer.Start();
        }
    }
}