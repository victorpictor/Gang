using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Clustering;
using Core.States.Services;
using NetMQ;
using Newtonsoft.Json;
using ZmqTransport.Settings;

namespace ZmqTransport.Discovery
{
    public class NodeDiscoveryService
    {
        private NodeSettings nodeSettings;

        private List<ClusterNode> ClusterNodes = new List<ClusterNode>();

        public NodeDiscoveryService(NodeSettings nodeSettings)
        {
            this.nodeSettings = nodeSettings;
        }

        public void Run()
        {
            new ServiceReference(BroadcastThisNodeSettings).StartService();

            DicoveryListener();
        }
        
        private void BroadcastThisNodeSettings()
        {
            var context = NetMQContext.Create();

            var disvoceryPort = new NetMQBeacon(context);
            disvoceryPort.Configure(DiscoverySettings.DiscoveryPort);

            disvoceryPort.Publish(JsonConvert.SerializeObject(nodeSettings.NodeDesctiption()));
        }

        private void DicoveryListener()
        {
            var context = NetMQContext.Create();

            using (var inbox = new NetMQBeacon(context))
            {
                inbox.Configure(DiscoverySettings.DiscoveryPort);
                inbox.Subscribe("");
                
                var startedDicovery = DateTime.Now;

                while (DateTime.Now.Subtract(startedDicovery).TotalSeconds < DiscoverySettings.DiscoveryPeriod)
                {
                    string peerName;
                    string message = inbox.ReceiveString(out peerName);
                    
                    var node = JsonConvert.DeserializeObject<ClusterNode>(message);

                    this.Info(string.Format("Discovered node {0}", node.Id));
                    
                    if (ClusterNodes.All(n => n.Id != node.Id))
                        ClusterNodes.Add(node);
                }
            }

            nodeSettings.ClusterNodes = ClusterNodes;
        }
    }
}