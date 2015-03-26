using System;
using Core;

namespace NodeHost
{
    public class nLogger:ILog
    {
        private NLog.Logger logger;

        public nLogger(NLog.Logger logger)
        {
            this.logger = logger;
        }

        public void Info(string message)
        {
            logger.Log(NLog.LogLevel.Info, message);
        }

        public void Exception(string message, Exception e)
        {
            logger.Log(NLog.LogLevel.Error, message);
        }
    }
}