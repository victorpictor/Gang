namespace Core.Clustering
{
    public class NodeStateService
    {
        private PersistentNodeState state;

        public NodeStateService()
        {
            //load from source
            state = new PersistentNodeState();
        }




    }
}