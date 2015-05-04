using Core.Clustering;
using Core.Log;
using Core.Messages;
using Core.Receivers;
using Core.Senders;

namespace Core
{
    public abstract class Configurator
    {
        protected ISendMessages nodeSender;
        protected ISendMessages controlMessageSender;

        protected IReceiveMessagesService nodeMessageReceiver;
        protected IReceiveMessages contolMessageQueue;
        protected IReceiveMessagesService clientCommandsReceiver;

        protected NodeSettings nodeSettings;

        protected NodeLogEntriesService logEntriesService;

        protected ILogEntryStore logEntryStore;
        

       
        public abstract DomainRegistry UseNodeMessageSender(ISendMessages sender);
        public abstract DomainRegistry UseControlMessageSender(ISendMessages sender);
        public abstract DomainRegistry UseNodeSettings(NodeSettings nodeSettings);
        public abstract DomainRegistry UseToReceiveMessages(IReceiveMessagesService nodeMessageReceiver);
        public abstract DomainRegistry UseToReceiveClientCommands(IReceiveMessagesService clientCommandsReceiver);
        public abstract DomainRegistry UseLogEntryStore(ILogEntryStore logEntryStore);
        public abstract DomainRegistry UseContolMessageQueue();
    }

}