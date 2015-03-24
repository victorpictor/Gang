using System;
using System.Collections;
using System.Threading;
using Core.Messages;
using Core.Receivers;
using Core.Senders;

namespace Core.Transport
{
    public class LeaderBus : IDeliverMessages
    {
        private Queue followerMessageQueue = Queue.Synchronized(new Queue());  


        private static IReceiveMessages<IClientCommand> commands;
        private static IReceiveMessages<IMessage> followerMessages;
        private static ISend<ClientReply> repliesBus;

        public static void InitLeaderBus(
            IReceiveMessages<IClientCommand> cs,
            ISend<ClientReply> rs,
            IReceiveMessages<IMessage> fms)
        {
            commands = cs;
            repliesBus = rs;
            followerMessages = fms;
        }

        public IClientCommand ReceiveCommand()
        {
            return new ClientCommand(){Id = Guid.NewGuid(), Command = new object()};
            return commands.Receive();
        }

        public IMessage ReceiveMessage()
        {

            while (true)
            {
                if (followerMessageQueue.Count > 0)
                    return (IMessage) followerMessageQueue.Dequeue();
                    
                Thread.Sleep(100);
            }
        }

        public void SendToClient()
        {
            repliesBus.Send(new ClientReply());
        }

        public void Send(IMessage message)
        {
        }

        public void Deliver(IMessage message)
        {
            followerMessageQueue.Enqueue(message);
        }
    }
}