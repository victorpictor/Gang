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
        private FinitState finitState;

        private ISendMessages sender;
        private IReceiveMessages receiver;

        public Node(NodeSettings settings, PersistentNodeState persistentNodeState, FinitState finitState, ISendMessages sender, IReceiveMessages receiver)
        {
            this.settings = settings;

            this.persistentNodeState = persistentNodeState;
            this.nodeState = new NodeState();
            this.finitState = finitState;

            this.sender = sender;
            this.receiver = receiver;
        }

        public void Start()
        {
            nodeState.EnterState(this, finitState, receiver);
        }

        public void Stop()
        {
            sender.Send(new ExitState());
        }

        public void Next(FinitState finitState)
        {
            nodeState = new  NodeState();
            this.finitState = finitState;

            nodeState.EnterState(this, finitState, receiver);
        }

       
        public PersistentNodeState GetState()
        {
            return this.persistentNodeState;
        }

        public FinitState LastFinitState()
        {
            return finitState;
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