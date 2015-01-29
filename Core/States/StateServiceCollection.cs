using System.Collections.Generic;
using Core.States.Services;

namespace Core.States
{
    public class StateServiceCollection
    {
        protected List<ServiceReference> registeredServices = new List<ServiceReference>();

        public void StartRegisteredServices()
        {
            registeredServices.ForEach(service => service.StartService());
        }

        public void StopRegisteredServices()
        {
            registeredServices.ForEach(service => service.StopService());
            registeredServices = new List<ServiceReference>();
        }

        public void RegisterService(ServiceReference service)
        {
            registeredServices.Add(service);
        }
    }
}