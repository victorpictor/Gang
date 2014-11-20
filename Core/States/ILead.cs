namespace Core.States
{
    public interface ILead
    {
        void SendHeartBeat();
        void SyncLog();
    }
}