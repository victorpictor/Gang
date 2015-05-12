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
    public class WenReplyingToMessage : Specification
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

            sender.Reply(new EntriesAppended(){To = nodeId});
            
            Thread.Sleep(1000);
            consumer.ConsumerService.StopService();
        }

        [Test]
        public void It_should_send_1_message_only()
        {
            Assert.AreEqual(1, queue.Count);
        }
    }
}