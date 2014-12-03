using Core.Messages;
using Core.Receivers;
using Core.Senders;

namespace Core.Transport
{
    public class LeaderBus
    {
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
            return commands.Receive();
        }

        public IMessage ReceiveMessage()
        {
            return followerMessages.Receive();
        }

        public void SendToClient()
        {
            repliesBus.Send(new ClientReply());
        }

        public void Send(IMessage message)
        {
        }
    }
}