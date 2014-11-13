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
        private FinitState nodeState;

        private ISendMessages sender;
        private IReceiveMessages receiver;

        public Node(NodeSettings settings, PersistentNodeState persistentNodeState, FinitState nodeState, ISendMessages sender, IReceiveMessages receiver)
        {
            this.settings = settings;

            this.persistentNodeState = persistentNodeState;
            this.nodeState = nodeState;

            this.sender = sender;
            this.receiver = receiver;
        }

        public void Start()
        {
            nodeState.EnterState(ref persistentNodeState, this);
        }

        public void Stop()
        {
            sender.Send(new ExitState());
        }

        public void Next(FinitState state)
        {
            nodeState = state;

            nodeState.EnterState(ref persistentNodeState, this);
        }

        public PersistentNodeState GetState()
        {
            return this.persistentNodeState;
        }
        public FinitState LastFinitState()
        {
            return nodeState;
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