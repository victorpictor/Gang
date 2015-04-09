using System;
using Core.Concurrency;
using Core.Messages;

namespace Core.States.TheLead
{
    public class RequestState
    {
        private DateTime lastMessageSent;
        private AppendEntriesReplies appendEntries = new AppendEntriesReplies();
        private bool failed = true;

        public RequestState()
        {
            lastMessageSent = DateTime.Now;
        }

        public void MessageSentNow()
        {
            lastMessageSent = DateTime.Now;
        }

        public DateTime LastMessageSent()
        {
            return lastMessageSent;
        }

        public void RegisterReply(EntriesAppended e)
        {
            appendEntries.Register(e);
        }

        public void FreshState()
        {
            
            failed = true;
            appendEntries = new AppendEntriesReplies();
        }


        public int IsMajority(long term, long index)
        {
            return appendEntries.Count(term, index);
        }

        public bool Failed()
        {
            return failed;
        }

        public void Succeeded()
        {
            failed = false;
        }

    }
}