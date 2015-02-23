using System;
using Core.Clustering;
using Core.Log;
using Core.Receivers;
using Core.Senders;

namespace Core
{
    public class DomainRegistry : Configurator, IDependencyFactory
    {
        public override DomainRegistry UseNodeMessageSender(ISendMessages sender)
        {
            this.nodeSender = sender;
            return this;
        }
        public override DomainRegistry UseDomainMessageSender(ISendMessages domainSender)
        {
            this.domainSender = domainSender;
            return this;
        }
        public override DomainRegistry UseNodeSettings(NodeSettings nodeSettings)
        {
            this.nodeSettings = nodeSettings;
            return this;
        }
        public override DomainRegistry UseToReceiveMessages(IReceiveMessages nodeMessageReceiver)
        {
            this.nodeMessageReceiver = nodeMessageReceiver;

            return this;
        }
        public override DomainRegistry UseLogEntryStore(ILogEntryStore logEntryStore)
        {
            this.logEntryStore = logEntryStore;
            this.logEntriesService = new NodeLogEntriesService(logEntryStore, nodeSettings.NodeId);
            
            return this;
        }

        public Receiver MessageReceiver()
        {
            if (nodeMessageReceiver == null)
                throw new NullReferenceException("No MessageReceiver registered");

            return new Receiver(nodeMessageReceiver);
        }
        public NodeLogEntriesService LogEntriesService()
        {
            if (logEntriesService == null)
                throw new NullReferenceException("No NodeLogEntriesService registered");

            return logEntriesService;
        }
        public ISendMessages NodeMessageSender()
        {
            if (nodeSender == null)
                throw new NullReferenceException("No NodeMessageSender registered");

            return nodeSender;
        }
        public ISendMessages DomainMessageSender()
        {
            if (domainSender == null)
                throw new NullReferenceException("No DomainMessageSender registered");

            return domainSender;
        }
        public NodeSettings NodeSettings()
        {
            if (nodeSettings == null)
                throw new NullReferenceException("No NodeSettings registered");

            return nodeSettings;
        }
    }
}