namespace Core.States
{
    public interface ILead
    {
        void HeartBeat();
        void SyncLog();
        void Appender();
    }
}