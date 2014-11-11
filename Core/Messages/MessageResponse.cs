using System;
using Core.Clustering;

namespace Core.Messages
{
    public class MessageResponse
    {
        public bool LeaveState;
        public Action<Node> Action;

        public MessageResponse(bool leaveState, Action<Node> next)
        {
            this.LeaveState = leaveState;
            this.Action = next;
        }
    }
}