using System;
using Core.Clustering;

namespace Core.Messages
{
    public class MessageResponse
    {
        public bool LeaveState;
        public Action Action;

        public MessageResponse(bool leaveState, Action next)
        {
            this.LeaveState = leaveState;
            this.Action = next;
        }
    }
}