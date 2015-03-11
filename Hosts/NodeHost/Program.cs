using System.Collections.Generic;
using System.Configuration;
using Core;
using Core.Clustering;
using Core.States.TheFollower;
using DataAccess;
using ZmqTransport.Discovery;
using ZmqTransport.MessageReceivers;
using ZmqTransport.MessageSenders;

namespace NodeHost
{
    class Program
    {
        
        static void Main(string[] args)
        {
           var nodeSettings = new FileNodeSettings().ReadSettings();

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
