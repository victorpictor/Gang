﻿using Core.Clustering;
using Core.Receivers;
using Core.Senders;

namespace Core
{
    public interface IDependencyFactory
    {
        Receiver MessageReceiver();
        NodeLogEntriesService LogEntriesService();
        ISendMessages NodeMessageSender();
        ISendMessages DomainMessageSender();
        NodeSettings NodeSettings();
    }

}