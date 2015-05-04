using Core;
using Core.Clustering;
using Core.States.TheFollower;
using DataAccess;
using NLog;
using ZmqTransport.Discovery;
using ZmqTransport.MessageReceivers;
using ZmqTransport.MessageSenders;
using ZmqTransport.Settings;

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
                .UseToReceiveClientCommands(new ClientCommandsReceiver(nodeSettings,new ClientSettigs()))
                .UseToReceiveMessages(new NodeMessageSubscriber(nodeSettings))
                .UseNodeMessageSender(new NodeMessageSender(nodeSettings))
                .UseLogEntryStore(new LogEntryStore());

         
           var node = new Node(new Follower(), registry);

           node.Start();
           
            while (true)
            {
                
            }

            node.Stop();
        }
    }
}
