﻿using System.Threading;
using Core.States.Services;

namespace Core.States.TheFollower
{
    public class LogRecoveryService : AbstractService 
    {
        public LogRecoveryService()
        {
            reference = new ServiceReference(() => { });
        }
    }
}