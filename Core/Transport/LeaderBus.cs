using System.Collections;
using System.Threading;
using Core.Messages;
using Core.Senders;

namespace Core.Transport
{
    public class LeaderBus
    {
        private Queue followerMessageQueue = Queue.Synchronized(new Queue());
        private Queue clientCommandsQueue = Queue.Synchronized(new Queue());  
        
        private static ISend<ClientReply> repliesBus;

        public static void InitLeaderBus(ISend<ClientReply> rs)
        {
            repliesBus = rs;
        }

        public IClientCommand ReceiveCommand()
        {
            while (true)
            {
                if (followerMessageQueue.Count > 0)
                    return (IClientCommand)clientCommandsQueue.Dequeue();

                Thread.Sleep(100);
            }
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

        public void Deliver(IMessage message)
        {
            followerMessageQueue.Enqueue(message);
        }

        public void Deliver(IClientCommand command)
        {
            clientCommandsQueue.Enqueue(command);
        }
    }
}