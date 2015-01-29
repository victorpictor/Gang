using System;
using System.Threading;

namespace Core.States.Services
{
    public class ServiceReference
    {
        private Thread serviceThread;

        public ServiceReference(Action a)
        {
            this.serviceThread = new Thread(new ThreadStart(a));
        }

        public void StopService()
        {
            if (serviceThread != null)
                if (serviceThread.IsAlive)
                    serviceThread.Abort();
        }

        public void StartService()
        {
            if (serviceThread != null)
                serviceThread.Start();
        }
    }
}