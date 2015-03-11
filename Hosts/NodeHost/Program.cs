using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Core;
using Core.Clustering;
using Core.Messages;
using Core.States.TheFollower;
using DataAccess;
using ZmqTransport.Discovery;
using ZmqTransport.MessageReceivers;
using ZmqTransport.MessageSenders;

namespace NodeHost
{
    class Program
    {
        private static NodeSettings ReadSettingsFile()
        {
            return new NodeSettings()
                {
                    NodeId = int.Parse(ConfigurationManager.AppSettings["NodeId"]),
                    NodeName = ConfigurationManager.AppSettings["NodeName"],
                    ElectionTimeout = int.Parse(ConfigurationManager.AppSettings["ElectionTimeout"]),
                    FollowerSla = int.Parse(ConfigurationManager.AppSettings["FollowerSla"]),
                    HeartBeatPeriod = int.Parse(ConfigurationManager.AppSettings["HeartBeatPeriod"]),
                    SubscribersPort = int.Parse(ConfigurationManager.AppSettings["SubscribersPort"]),
                    ClusterNodes = new List<ClusterNode>(),
                    Majority = 2
                };
        }

        static void Main(string[] args)
        {

           var nodeSettings = ReadSettingsFile();

           new NodeDiscoveryService(nodeSettings).Run();

           var registry =
                new DomainRegistry()
                .UseNodeSettings(nodeSettings)
                .UseContolMessageQueue()
                .UseToReceiveMessages(new NodeMessageSubscriber(nodeSettings))
                .UseNodeMessageSender(new NodeMessageSender(nodeSettings))
                .UseLogEntryStore(new LogEntryStore());


            var node = new Node(new Follower(), registry);

            node.Start();

            
            node.Stop();
        }
    }
}
