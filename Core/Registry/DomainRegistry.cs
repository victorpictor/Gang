using System;
using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.Receivers;
using Core.Senders;

namespace Core
{
    public class DomainRegistry : Configurator
    {
        public override DomainRegistry UseNodeMessageSender(ISendMessages sender)
        {
            this.nodeSender = sender;
            return this;
        }
        
        public override DomainRegistry UseNodeSettings(NodeSettings nodeSettings)
        {
            this.nodeSettings = nodeSettings;
            return this;
        }

        public override DomainRegistry UseToReceiveMessages(IReceiveMessagesService nodeMessageReceiver)
        {
            this.nodeMessageReceiver = nodeMessageReceiver;

            return this;
        }

        public override DomainRegistry UseToReceiveClientCommands(IReceiveMessagesService clientCommandsReceiver)
        {
            this.clientCommandsReceiver = clientCommandsReceiver;

            return this;
        }

        public override DomainRegistry UseControlMessageSender(ISendMessages sender)
        {
            this.controlMessageSender = sender;

            return this;
        }

        public override DomainRegistry UseLogEntryStore(ILogEntryStore logEntryStore)
        {
            this.logEntryStore = logEntryStore;
            this.logEntriesService = new NodeLogEntriesService(logEntryStore, nodeSettings.NodeId);
            
            return this;
        }

        public override DomainRegistry UseContolMessageQueue()
        {
            this.contolMessageQueue = new ContolMessageQueue();

            return this;
        }

        public Receiver MessageReceiver()
        {
            if (contolMessageQueue == null)
                throw new NullReferenceException("No ContolMessageQueue registered");

            if (nodeMessageReceiver == null)
                throw new NullReferenceException("No MessageReceiver registered");

            return new Receiver((IReceiveMessages)nodeMessageReceiver, contolMessageQueue);
        }

        public IReceiveMessagesService ClientCommandsReceiver()
        {
            if (clientCommandsReceiver == null)
                throw new NullReferenceException("No ClientCommandsReceiver registered");

            return this.clientCommandsReceiver;
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
        public ISendMessages ContolMessageSender()
        {
            if (controlMessageSender != null)
                return controlMessageSender;

            if (contolMessageQueue == null)
                throw new NullReferenceException("No ContolMessageQueue registered");

            return new ContolMessageSender((IDeliver<IMessage>)contolMessageQueue);
        }
        public NodeSettings NodeSettings()
        {
            if (nodeSettings == null)
                throw new NullReferenceException("No NodeSettings registered");

            return nodeSettings;
        }
    }
}