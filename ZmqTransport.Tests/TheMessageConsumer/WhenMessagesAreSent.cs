using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Core.Clustering;
using Core.Messages;
using NUnit.Framework;
using ZmqTransport.MessageReceivers;
using ZmqTransport.Tests.Consumer;

namespace ZmqTransport.Tests.TheMessageConsumer
{
    [TestFixture]
    public class WhenMessagesAreSent : Specification
    {
        private MessageConsumer messageConsumer1;
        
        private TestSender sender;
        private Queue messageQueue1 = Queue.Synchronized(new Queue());
        
        public override void Given() 
        {
            messageConsumer1 = new MessageConsumer(900, messageQueue1, 1);
            sender = new TestSender(new NodeSettings() { SubscribersPort = 900, ClusterNodes = new List<ClusterNode>() { new ClusterNode() { Id = 1} } });
        }

        public override void When()
        {
            messageConsumer1.StartConsume();
           
            sender.Send(new AppendEntries(){MachineCommands = new List<object>()});

            messageConsumer1.StopConsume();
        }

        [Test]
        public void It_should_receive()
        {
            Assert.AreEqual(2, messageQueue1.Count);
        }
    }
}