using Core.Clustering;
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

        public abstract DomainRegistry UseNodeMessageSender(ISendMessages sender);
        public abstract DomainRegistry UseDomainMessageSender(ISendMessages domainSender);
        public abstract DomainRegistry UseNodeSettings(NodeSettings nodeSettings);
        public abstract DomainRegistry UseNodeLogEntriesService(NodeLogEntriesService logEntriesService);
        public abstract DomainRegistry UseToReceiveMessages(IReceiveMessages nodeMessageReceiver);
    }

}