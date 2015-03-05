using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Clustering;
using Core.Messages;
using Core.States.TheFollower;
using DataAccess;
using ZmqTransport.MessageReceivers;
using ZmqTransport.MessageSenders;

namespace NodeHost
{
    class Program
    {
        static void Main(string[] args)
        {
           var registry =
                new DomainRegistry()
                .UseNodeSettings(new NodeSettings(){NodeId = 1, NodeName = "first", ElectionTimeout = 3000, FollowerSla = 200, HeartBeatPeriod = 150, KnownNodes = new List<int>() {2, 3, 4}, Majority = 3})
                .UseToReceiveMessages(new SubscriberNodeMessageReceiver())
                .UseNodeMessageSender(new NodeMessageSender())
                .UseContolMessageSender(new DomainMessageSender())
                .UseLogEntryStore(new LogEntryStore());


            var node = new Node(new Follower(), registry);

            node.Start();

            while (true)
            {
                var command = Console.ReadLine();
                if (command == "exit")
                    break;
            }

            node.Stop();
        }
    }
}
