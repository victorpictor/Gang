using System;
using System.Threading;

namespace Core.States.Services
{
    public class ServiceReference
    {
        private Thread serviceThread;

        private bool IsShuttingDown;

        private object _lock = new object();

        public ServiceReference(Action a)
        {
            this.serviceThread = new Thread(new ThreadStart(a));
        }

        public void StopService()
        {
            lock (_lock)
            {
                IsShuttingDown = true;
            }
        }

        public bool IsServiceShuttingDown()
        {
            var isShuttingDown = false;

            lock (_lock)
            {
                isShuttingDown = IsShuttingDown;
            }

            return isShuttingDown;
        }

        public void StartService()
        {
            if (serviceThread != null)
               serviceThread.Start();
        }
    }
}