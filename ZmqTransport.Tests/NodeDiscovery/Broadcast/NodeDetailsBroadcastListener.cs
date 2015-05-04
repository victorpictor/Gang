using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Clustering;
using NetMQ;
using Newtonsoft.Json;
using ZmqTransport.Settings;

namespace ZmqTransport.Tests.NodeDiscovery
{
    public class NodeDetailsBroadcastListener
    {
        public List<ClusterNode> ClusterNodes = new List<ClusterNode>();

        public void ListenForNodes()
        {
            var context = NetMQContext.Create();

            using (var inbox = new NetMQBeacon(context))
            {
                inbox.Configure(DiscoverySettings.DiscoveryPort);
                inbox.Subscribe("");

                string peerName;
                string message = inbox.ReceiveString(out peerName);

                var node = JsonConvert.DeserializeObject<ClusterNode>(message);

                this.Info(string.Format("Discovered node {0}", node.Id));

                if (ClusterNodes.All(n => n.Id != node.Id))
                    ClusterNodes.Add(node);
                
            }
        } 
    }
}