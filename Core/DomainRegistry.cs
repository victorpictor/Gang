using Core.Clustering;

namespace Core
{
    public interface IServiceFactory
    {
        NodeLogEntriesService NodLogEntriesService();
    }
    
    public class DomainRegistry
    {
        static NodeLogEntriesService nodeLogEntriesService;

        public static void RegisterService(NodeLogEntriesService _nodeLogEntriesService)
        {
            nodeLogEntriesService = _nodeLogEntriesService;
        }
        
        public static NodeLogEntriesService NodLogEntriesService()
        {
            return nodeLogEntriesService;
        }
    }
}