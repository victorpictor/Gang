using System;
using Core.Clustering;

namespace Core.States
{
    public class Stoped : FinitState
    {
        public override void EnterState(ref PersistentNodeState persistentNodeState, Node node)
        {
            Console.WriteLine("Node stopped {0}", persistentNodeState.NodeId);
        }
    }
}