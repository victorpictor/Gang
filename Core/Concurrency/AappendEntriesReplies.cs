using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Messages;

namespace Core.Concurrency
{
    public class AappendEntriesReplies
    {
        static ReaderWriterLock Lock = new ReaderWriterLock();
        protected HashSet<EntriesAppended> appendEntriesResponses = new HashSet<EntriesAppended>();

        public void Register(EntriesAppended e)
        {
            Lock.AcquireWriterLock(10);

            if (!appendEntriesResponses.Any(
                       ae => ae.NodeId == e.NodeId &&
                           ae.Term == e.Term &&
                           ae.LogIndex == e.LogIndex))
                appendEntriesResponses.Add(e);

            Lock.ReleaseWriterLock();
        }

        public int Count(long term, long index)
        {
            var resp = 0;

            Lock.AcquireReaderLock(100);

            resp = appendEntriesResponses.Count(ae => ae.Term == term && ae.LogIndex == index);

            Lock.ReleaseReaderLock();

            return resp;
        }
    }
}