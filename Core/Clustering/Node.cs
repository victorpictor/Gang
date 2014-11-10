using Core.States;

namespace Core.Clustering
{
    public class Node
    {
        private string nodeName;

        private LogState logState;

        private FinitState nodeState;

        public Node(string nodeName, LogState logState, FinitState nodeState)
        {
            this.nodeName = nodeName;
            this.logState = logState;
            this.nodeState = nodeState;
        }



    }
}