using Core.Messages;

namespace Core.Receivers
{
    public class Receiver
    {
        private IReceiveMessages receiveMessages;
        private IReceiveMessages receiveControlMessages;

        public Receiver(IReceiveMessages receiveMessages, IReceiveMessages receiveControlMessages)
        {
            this.receiveMessages = receiveMessages;
            this.receiveControlMessages = receiveControlMessages;
        }

        public IMessage NextMessage()
        {
            while (true)
            {
                var message = receiveControlMessages.Receive();
                if (message.Term > 0)
                    return message;

                message = receiveMessages.Receive();
                if (message.Term >= 0)
                    return message;
            }
        } 
    }
}