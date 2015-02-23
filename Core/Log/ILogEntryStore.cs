namespace Core.Log
{
    public interface ILogEntryStore
    {
        void Append(LogEntry le);
        LogEntry LastAppended();
    }
}