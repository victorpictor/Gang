using Core.Clustering;
using NetMQ;
using Newtonsoft.Json;

namespace ZmqTransport.Tests.NodeDiscovery
{
    public class NodeDetailsBroadcaster
    {
        public void Broadcast(ClusterNode node, int discoveryPort)
        {
            var context = NetMQContext.Create();

            var disvoceryPort = new NetMQBeacon(context);
            disvoceryPort.Configure(discoveryPort);

            disvoceryPort.Publish(JsonConvert.SerializeObject(node));
        } 
    }
}