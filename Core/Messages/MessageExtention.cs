using System.Linq;

namespace Core.Messages
{
    public static class MessageExtention
    {
        public static bool IsHeartBeat(this AppendEntries appendEntries)
        {
            return !appendEntries.Messages.Any();
        }
    }
}