using System.Collections.Generic;
using System.Configuration;
using Core.Clustering;

namespace NodeHost
{
    public class FileNodeSettings
    {
        public  NodeSettings ReadSettings()
        {
            return new NodeSettings()
            {
                NodeId = int.Parse(ConfigurationManager.AppSettings["NodeId"]),
                NodeName = ConfigurationManager.AppSettings["NodeName"],
                ElectionTimeout = int.Parse(ConfigurationManager.AppSettings["ElectionTimeout"]),
                FollowerTimeout = int.Parse(ConfigurationManager.AppSettings["FollowerTimeout"]),
                FollowerSla = int.Parse(ConfigurationManager.AppSettings["FollowerSla"]),
                HeartBeatPeriod = int.Parse(ConfigurationManager.AppSettings["HeartBeatPeriod"]),
                SubscribersPort = int.Parse(ConfigurationManager.AppSettings["SubscribersPort"]),
                ClusterNodes = new List<ClusterNode>(),
                Majority = 2
            };
        }

    }
}