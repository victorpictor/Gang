using System;
using System.Collections.Generic;
using System.Linq;
using Core.Messages;
using Newtonsoft.Json;

namespace ZmqTransport
{
    public class MessageRegistry
    {
        private List<Type> KnownMessages = new List<Type>();

        public MessageRegistry()
        {
            var commandType = typeof(IMessage);

            KnownMessages = AppDomain.CurrentDomain.GetAssemblies()
                                 .SelectMany(s => s.GetTypes())
                                 .Where(p => commandType.IsAssignableFrom(p) && p.IsClass).ToList();
        }

        public dynamic Create(string type, string sMessage)
        {
            var ct = KnownMessages.Where(t => t.Name == type || t.FullName == type).Select(tt => tt).FirstOrDefault();

            return JsonConvert.DeserializeObject(sMessage, ct);
        }
    }
   
}