using System.Collections.Generic;
using Core.Clustering;
using Core.Log;
using Moq;
using NUnit.Framework;

namespace Core.Specs.WhenAppendingToLog
{
    [TestFixture]
    public class AndCommitting : Specification
    {
        private PersistentNodeState state;
        private ILogEntryStore store;

        public override void Given()
        {
            store = new LogEntryStore();
            state = new PersistentNodeState()
                {
                    NodeId = 1,
                    Term = 2,
                    EntryIndex = 1,
                    PendingCommit = new LogEntry() {Term = 2, Index = 1, MachineCommands = new List<object>() {new object()}}
                };
        }

        public override void When()
        {
            state.Append(2, 2, 2, 1, new List<object>() { new object() }, store);
        }


        [Test]
        public void It_should_append_new_entries()
        {
            Assert.AreEqual(2, state.EntryIndex);
            Assert.AreEqual(2, state.Term);
        }

        [Test]
        public void It_should_commit_prev()
        {
            Assert.AreEqual(1, ((LogEntryStore)store).Count());

        }

        [Test]
        public void It_should_save_pending_commit()
        {
            Assert.AreEqual(2, state.PendingCommit.Term);
            Assert.AreEqual(2, state.PendingCommit.Index);
        }

    }
}