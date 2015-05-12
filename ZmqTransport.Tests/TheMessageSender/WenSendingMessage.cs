using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using NUnit.Framework;
using ZmqTransport.MessageSenders;

namespace ZmqTransport.Tests.TheMessageSender
{
    [TestFixture]
    public class WenSendingMessage : Specification
    {
        private NodeMessageSender sender;
        private TestConsumer consumer;
        private Queue queue = Queue.Synchronized(new Queue());

        private int nodeId = 2;
        private int consumerPort = 4041;

        public override void Given()
        {
            sender = new NodeMessageSender(new NodeSettings() { NodeId = 1, SubscribersPort = consumerPort, ClusterNodes = new List<ClusterNode>() { new ClusterNode() { Id = nodeId } } });      
        
            consumer = new TestConsumer(consumerPort, queue, nodeId);
        }

        public override void When()
        {
            consumer.ConsumerService.StartService();
            
            sender.Send(new AppendEntries(){});
            sender.Send(new AppendEntries());
            sender.Send(new AppendEntries());
            sender.Send(new AppendEntries());

            Thread.Sleep(1000);
            consumer.ConsumerService.StopService();
        }

        [Test]
        public void It_should_send_4_messages()
        {
            Assert.AreEqual(4, queue.Count);
        }


    }
}