using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Clustering;
using Core.States.Services;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.Discovery
{
    public class NodeDiscoveryService
    {
        private NodeSettings nodeSettings;

        public List<ClusterNode> ClusterNodes = new List<ClusterNode>();

        public NodeDiscoveryService(NodeSettings nodeSettings)
        {
            this.nodeSettings = nodeSettings;
        }

        public void Run()
        {
            new ServiceReference(SettingPublisher).StartService();

            DicoveryInbox();
        }
        
        private void SettingPublisher()
        {
            var context = NetMQContext.Create();

            var disvoceryPort = new NetMQBeacon(context);
            disvoceryPort.Configure(9999);

            disvoceryPort.Publish(JsonConvert.SerializeObject(nodeSettings.NodeDesctiption()));
        }

        private void DicoveryInbox()
        {
            var context = NetMQContext.Create();

            using (var inbox = new NetMQBeacon(context))
            {
                inbox.Configure(9999);
                inbox.Subscribe("");
                
                var startedDicovery = DateTime.Now;

                while (DateTime.Now.Subtract(startedDicovery).TotalSeconds < nodeSettings.ClusterDiscoveryPeriod)
                {
                    string peerName;
                    string message = inbox.ReceiveString(out peerName);
                    
                    var node = JsonConvert.DeserializeObject<ClusterNode>(message);

                    this.Info(string.Format("Discovered node node {0}", node.Id));
                    
                    if (ClusterNodes.All(n => n.Id != node.Id))
                        ClusterNodes.Add(node);
                }
            }

            nodeSettings.ClusterNodes = ClusterNodes;

            //if (ClusterNodes.Count < 2 * nodeSettings.Majority - 1)
            //{
            //    throw new Exception("Could not join a cluster with a majority of " + nodeSettings.Majority);
            //}

        }
    }
}