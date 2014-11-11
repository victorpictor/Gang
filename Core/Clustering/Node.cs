using Core.States;

namespace Core.Clustering
{
    public class Node
    {
        private NodeSettings settings;

        private PersistentNodeState persistentNodeState;

        private FinitState nodeState;

        public Node(NodeSettings settings, PersistentNodeState persistentNodeState, FinitState nodeState)
        {
            this.settings = settings;
            this.persistentNodeState = persistentNodeState;
            this.nodeState = nodeState;
        }

        public void Start()
        {
            nodeState.EnterState(ref persistentNodeState, this);
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
        
    }
}