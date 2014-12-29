using Core.Clustering;

namespace Core.Specs
{
    public class ServiceFactory : IServiceFactory
    {
        private NodLogEntriesService service;

        public ServiceFactory(PersistentNodeState state)
        {
            this.service = new NodLogEntriesService(state);
        }

        public NodLogEntriesService NodLogEntriesService()
        {
            return service;
        }
    }
}