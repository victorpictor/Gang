using System;
using Core.Clustering;

namespace Core.Specs
{
    public class ServiceFactory : IServiceFactory
    {
        private static NodeLogEntriesService service;

        public ServiceFactory(PersistentNodeState state)
        {
            service = new NodeLogEntriesService(state);
        }

        public NodeLogEntriesService NodLogEntriesService()
        {
            return service;
        }
    }
}