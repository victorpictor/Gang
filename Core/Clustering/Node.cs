using Core.States;

namespace Core.Clustering
{
    public class Node
    {
        private NodeSettings settings;

        private LogState logState;

        private FinitState nodeState;

        public Node(NodeSettings settings, LogState logState, FinitState nodeState)
        {
            this.settings = settings;
            this.logState = logState;
            this.nodeState = nodeState;
        }

        public void Start()
        {
            nodeState.EnterState(ref logState, this);
        }

        public void Next(FinitState state)
        {
            nodeState = state;

            nodeState.EnterState(ref logState, this);
        }

    }
}