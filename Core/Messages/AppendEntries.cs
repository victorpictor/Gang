using System.Collections.Generic;

namespace Core.Messages
{
    public class AppendEntries : IMessage
    {
        public AppendEntries()
        {
        }

        public AppendEntries(long term, long logIndex, List<object> machineCommands)
        {
            Term = term;
            LogIndex = logIndex;
            MachineCommands = machineCommands;
        }

        public AppendEntries(int leaderId, long term, long prevTerm, long logIndex, long prevLogIndex, List<object> machineCommands)
        {
            LeaderId = leaderId;
            Term = term;
            PrevTerm = prevTerm;
            LogIndex = logIndex;
            PrevLogIndex = prevLogIndex;
            MachineCommands = machineCommands;
        }

        public int LeaderId { get; set; }
        public long Term { get; set; }
        public long PrevTerm { get; set; }
        public long PrevLogIndex { get; set; }
        public long LogIndex { get; set; }

        public List<object> MachineCommands { get; set; }

        
    }

   
}