using System;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class Leader : FinitState, ILead
    {
        public override void EnterState(Node node)
        {
            base.EnterState(node);
        }

        
        public void SendHeartBeat()
        {
        }

        public void SyncLog()
        {
            
        }
    }
}