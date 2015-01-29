using Core.Messages;

namespace Core.Receivers
{
    public class Receiver
    {
        private IReceiveMessages receiveMessages;

        public Receiver(IReceiveMessages receiveMessages)
        {
            this.receiveMessages = receiveMessages;
        }

        public IMessage NextMessage()
        {
            while (true)
            {
                var message = receiveMessages.Receive();

                if (message.Term >= 0)
                    return message;
            }
        } 
    }
}