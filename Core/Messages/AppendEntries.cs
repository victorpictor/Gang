﻿using System.Collections.Generic;
using System.Linq;

namespace Core.Messages
{
    public class AppendEntries : IMessage
    {
        public int LeaderId { get; set; }
        public long Term { get; set; }
        public long PrevTerm { get; set; }
        public long PrevLogIndex { get; set; }
        public long LogIndex { get; set; }

        public List<object> Messages { get; set; }

        
    }
}