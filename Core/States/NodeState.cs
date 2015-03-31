using System;
using System.Threading;
using Core.Clustering;
using Core.Messages;

namespace Core.States
{
    public class NodeState
    {
        private Node node;
        private DomainRegistry registry;
        private FinitState finitState;

        public NodeState(Node node, FinitState state, DomainRegistry registry)
        {
            this.node = node;
            this.registry = registry;
            this.finitState = state;
        }

        public FinitState ReadState()
        {
            return finitState;
        }

        public virtual void EnterState()
        {
            finitState.EnterNewState(node);
            
            var loop = new Thread(() =>
            {
                try
                {
                    var receiver = registry.MessageReceiver();
                    MessageResponse msgResp;

                    while (true)
                    {
                        var message = receiver.NextMessage();

                        msgResp = finitState.Receive((dynamic)message);

                        if (msgResp.LeaveState)
                            break;

                        msgResp.Action();
                    }

                    finitState.Transition(() => msgResp.Action());
                }
                catch (Exception e)
                {
                    this.Error(string.Format("Exiting the loop with exception"), e);
                }
                
            });

            loop.Start();
        }

        
    }
}