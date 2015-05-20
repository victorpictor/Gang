using System;
using Core.Clustering;
using Core.Messages;
using Core.Messages.Control;
using Core.States.Services;
using Core.Transport;

namespace Core.States.TheLead
{
    public class ClientCommandsService : AbstractService
    {
        public ClientCommandsService(Node node, LeaderBus leaderBus)
        {
            var receiver = node.GetRegistry().ClientCommandsReceiver();

            Action deliveryLoop = () =>
                {
                    receiver.StartService();

                    while (IsServiceShuttingDown())
                    {
                        var message = receiver.Receive();

                        if (message is NoMessageInQueue) continue;
                        
                        leaderBus.Deliver((IClientCommand)message);
                    }
                    receiver.StopService();
                };

            reference = new ServiceReference(deliveryLoop);
        }
    }
}