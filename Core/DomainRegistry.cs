using System;
using Core.Clustering;
using Core.Receivers;
using Core.Senders;

namespace Core
{
    public class DomainRegistry
    {
        private ISendMessages nodeSender;
        private ISendMessages domainSender;
        private IReceiveMessages nodeMessageReceiver;
        private NodeSettings nodeSettings;
        private NodeLogEntriesService logEntriesService;

        public DomainRegistry UseNodeMessageSender(ISendMessages sender)
        {
            this.nodeSender = sender;
            return this;
        }
        
        public DomainRegistry UseDomainMessageSender(ISendMessages domainSender)
        {
            this.domainSender = domainSender;
            return this;
        }

        public DomainRegistry UseNodeSettings(NodeSettings nodeSettings)
        {
            this.nodeSettings = nodeSettings;
            return this;
        }

        public DomainRegistry UseNodeLogEntriesService(NodeLogEntriesService logEntriesService)
        {
            this.logEntriesService = logEntriesService;

            return this;
        }

        public DomainRegistry UseToReceiveMessages(IReceiveMessages nodeMessageReceiver)
        {
            this.nodeMessageReceiver = nodeMessageReceiver;
            
            return this;
        }

        public IReceiveMessages MessageReceiver()
        {
            if (nodeMessageReceiver == null)
                throw new NullReferenceException("No MessageReceiver registered");

            return nodeMessageReceiver;
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