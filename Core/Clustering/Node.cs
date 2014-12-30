using Core.Messages;
using Core.Receivers;
using Core.Senders;
using Core.States;

namespace Core.Clustering
{
    public class Node
    {
        // -> to domain registry
        private NodeSettings settings;
        //

        private NodeState nodeState;
        private NodeLogEntriesService logEntriesService;

        private ISendMessages sender;
        private IReceiveMessages receiver;

        public Node(NodeSettings settings, FinitState finitState, NodeLogEntriesService logEntriesService, ISendMessages sender, IReceiveMessages receiver)
        {
            this.settings = settings;
            this.logEntriesService = logEntriesService;

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

        public NodeLogEntriesService NodLogEntriesService()
        {
            return logEntriesService;
        }

        public void Next(FinitState finitState)
        {
            nodeState = new NodeState(this, finitState, receiver);
           
            nodeState.EnterState();
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