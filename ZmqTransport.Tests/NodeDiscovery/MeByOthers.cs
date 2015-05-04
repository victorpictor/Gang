using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using NUnit.Framework;
using ZmqTransport.Discovery;

namespace ZmqTransport.Tests.NodeDiscovery
{
    [TestFixture]
    public class MeByOthers :Specification
    {
        private NodeDiscoveryService nodeDiscoveryService;
        private NodeSettings nodeSettings;
        private NodeDetailsBroadcastListener broadcastListener;

        public override void Given()
        {
            nodeSettings = new NodeSettings() {NodeId = 1, NodeName = "node1", SubscribersPort = 81};
            nodeDiscoveryService = new NodeDiscoveryService(nodeSettings);

            broadcastListener = new NodeDetailsBroadcastListener();
        }

        public override void When()
        {
           for (int i = 0; i < 5; i++)
               nodeDiscoveryService.Run();

            broadcastListener.ListenForNodes();
        }

        [Test]
        public void Listener_should_receive_my_details()
        {
            Assert.AreEqual(1,broadcastListener.ClusterNodes.Count);
        }

    }
}