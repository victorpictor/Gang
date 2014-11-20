using Core.Clustering;

namespace Core.States
{
    public class Leader : FinitState, ILead
    {
        public override void EnterState(Node node)
        {
            
        }

        public void SendHeartBeat()
        {
        }

        public void SyncLog()
        {
            
        }
    }
}