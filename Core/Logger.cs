using System;

namespace Core
{
    public interface ILog
    {
        void Info(string message);
        void Exception(string message, Exception e);
    }

    public static class Logger
    {
        private static ILog log;

        public static void Set(ILog _log)
        {
            log = _log;
        }

        public static void Info(this object o, string message)
        {
            log.Info(message);
        }

        public static void Error(this object o, string message, Exception e)
        {
            log.Error(message, e);
        }
    }
} 