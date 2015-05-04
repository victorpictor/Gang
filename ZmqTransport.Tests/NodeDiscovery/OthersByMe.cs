using Core.Clustering;
using NUnit.Framework;
using ZmqTransport.Discovery;
using ZmqTransport.Settings;

namespace ZmqTransport.Tests.NodeDiscovery
{
    public class OthersByMe :Specification
    {
        private NodeSettings nodeSettings;
        private NodeDetailsBroadcaster broadcaster;
        private NodeDiscoveryService nodeDiscoveryService;

        public override void Given()
        {
            nodeSettings = new NodeSettings() { NodeId = 1, NodeName = "node1", SubscribersPort = 81 };
            nodeDiscoveryService = new NodeDiscoveryService(nodeSettings);

            broadcaster = new NodeDetailsBroadcaster();
        }

        public override void When()
        {
           broadcaster.Broadcast(new ClusterNode() { Id = 3, Name = "otherNode", SubscriberPort = 82, Topic = "node1" }, DiscoverySettings.DiscoveryPort);
                
           nodeDiscoveryService.Run();
        }

        [Test]
        public void I_should_receive_other_node_details()
        {
            Assert.AreEqual(1, nodeSettings.ClusterNodes.Count);

            var otherNode = nodeSettings.ClusterNodes[0];

            Assert.AreEqual(3, otherNode.Id);
            Assert.AreEqual("otherNode", otherNode.Name);
            Assert.AreEqual(82, otherNode.SubscriberPort);
            Assert.AreEqual("node1", otherNode.Topic);
        }
    }
}