using System;
using Core.Clustering;

namespace Core.States
{
    public class Stoped : FinitState
    {
        public override void EnterState(Node node)
        {
            Console.WriteLine("Node stopped {0}", node.GetState().NodeId); 
        }
    }
}