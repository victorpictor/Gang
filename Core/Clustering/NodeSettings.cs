using System.Collections.Generic;

namespace Core.Clustering
{
    public class NodeSettings
    {
        public string NodeName;

        public int NodeId;
        
        public int ElectionTimeout = 1000;

        public int Majority = 3;

        public List<int> KnownNodes = new List<int>();
    }
}