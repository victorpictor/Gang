namespace Core.States.Services
{
    public class AbstractService
    {
        protected ServiceReference reference;

        public AbstractService StartService()
        {
            reference.StartService();

            return this;
        }

        public ServiceReference Reference()
        {
            return reference;
        }
    }
}