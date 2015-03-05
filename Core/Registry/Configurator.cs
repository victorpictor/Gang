using Core.Clustering;
using Core.Log;
using Core.Receivers;
using Core.Senders;

namespace Core
{
    public abstract class Configurator
    {
        protected ISendMessages nodeSender;
        protected ISendMessages domainSender;
        protected IReceiveMessages nodeMessageReceiver;
        protected NodeSettings nodeSettings;
        protected NodeLogEntriesService logEntriesService;
        protected ILogEntryStore logEntryStore;
        protected IReceiveMessages contolMessageQueue;
       
        public abstract DomainRegistry UseNodeMessageSender(ISendMessages sender);
        public abstract DomainRegistry UseContolMessageSender(ISendMessages domainSender);
        public abstract DomainRegistry UseNodeSettings(NodeSettings nodeSettings);
        public abstract DomainRegistry UseToReceiveMessages(IReceiveMessages nodeMessageReceiver);
        public abstract DomainRegistry UseLogEntryStore(ILogEntryStore logEntryStore);
        public abstract DomainRegistry UseContolMessageQueue();
    }

}