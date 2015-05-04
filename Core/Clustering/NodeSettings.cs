using System.Collections.Generic;

namespace Core.Clustering
{
    public class NodeSettings
    {
        public string NodeName;

        public int NodeId;
        
        public int ElectionTimeout;
        public int FollowerTimeout;
        public int HeartBeatPeriod;
        public int FollowerSla;
        public int Majority;
        public int SubscribersPort;
        public int ClusterDiscoveryPeriod;

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