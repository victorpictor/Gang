using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Core;
using Core.Clustering;
using Core.States.TheFollower;
using DataAccess;
using NLog;
using ZmqTransport.Discovery;
using ZmqTransport.MessageReceivers;
using ZmqTransport.MessageSenders;

namespace NodeHost
{
    class Program
    {
        static void Main(string[] args)
        {

            var logger = LogManager.GetCurrentClassLogger();
            Core.Logger.Set(new nLogger(logger));

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
           
            while (true)
            {
                
            }

          // node.Stop();
        }
    }
}
