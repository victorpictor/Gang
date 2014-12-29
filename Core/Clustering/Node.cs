using Core.Messages;
using Core.Receivers;
using Core.Senders;
using Core.States;

namespace Core.Clustering
{
    public class Node
    {
        private NodeSettings settings;

        private PersistentNodeState persistentNodeState;
        private NodeState nodeState;
       
        private ISendMessages sender;
        private IReceiveMessages receiver;

        public Node(NodeSettings settings, FinitState finitState, ISendMessages sender, IReceiveMessages receiver)
        {
            this.settings = settings;

            this.nodeState = new NodeState(this, finitState, receiver);
            
            this.sender = sender;
            this.receiver = receiver;
        }

        public void Start()
        {
            nodeState.EnterState();
        }

        public void Stop()
        {
            sender.Send(new ExitState());
        }

        public void Next(FinitState finitState)
        {
            nodeState = new NodeState(this, finitState, receiver);
           
            nodeState.EnterState();
        }
       
        public PersistentNodeState GetState()
        {
            return this.persistentNodeState;
        }

        public FinitState LastFinitState()
        {
            return nodeState.ReadState();
        }
        public NodeSettings GetSettings()
        {
            return this.settings;
        }

        public void Send(IMessage message)
        {
            sender.Send(message);
        }

        public IMessage Receive()
        {
            return receiver.Receive();
        }
    }
}