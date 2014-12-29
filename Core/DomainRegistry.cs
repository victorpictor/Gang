using System;
using Core.Clustering;

namespace Core
{
    public interface IServiceFactory
    {
        NodLogEntriesService NodLogEntriesService();
    }


    public class DomainRegistry
    {
        static IServiceFactory serviceFactory;

        public static void RegisterServiceFactory(IServiceFactory _serviceFactory)
        {
            serviceFactory = _serviceFactory;
        }
        
        public static NodLogEntriesService NodLogEntriesService()
        {
            return serviceFactory.NodLogEntriesService();
        }
    }
}