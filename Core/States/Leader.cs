﻿using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Leader : FinitState, ILead
    {
        public void EnterState(ref PersistentNodeState persistentNodeState, Node node)
        {
            throw new System.NotImplementedException();
        }

        public void Receive(AppendEntries appendEntries)
        {
            throw new System.NotImplementedException();
        }

        public void Receive(RequestedVote requestedVote)
        {
            throw new System.NotImplementedException();
        }

        public void Receive(VoteGranted voteGranted)
        {
            throw new System.NotImplementedException();
        }

        public void SendHeartBeat()
        {
            throw new System.NotImplementedException();
        }
    }
}