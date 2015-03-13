using System.Collections.Generic;

namespace Core.Clustering
{
    public class NodeSettings
    {
        public string NodeName;

        public int NodeId;
        
        public int ElectionTimeout = 1000;
        public int FollowerTimeout = 1000;
        public int HeartBeatPeriod = 1000;
        public int FollowerSla = 1000;
        public int Majority = 3;
        public int SubscribersPort = 8181;
        public int ClusterDiscoveryPeriod = 3;

        public List<ClusterNode> ClusterNodes = new List<ClusterNode>();

        public ClusterNode NodeDesctiption()
        {
            return new ClusterNode()
                        {
                            Id = this.NodeId,
                            Name = this.NodeName,
                            Topic = "topic" + this.NodeId,
                            SubscriberPort = this.SubscribersPort
                        };
        }
    }
}