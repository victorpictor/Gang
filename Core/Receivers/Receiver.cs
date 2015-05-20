using Core.Messages;
using Core.Messages.Control;

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
                if (!(message is NoMessageInQueue))
                    return message;

                message = receiveMessages.Receive();
                if (!(message is NoMessageInQueue))
                    return message;
            }
        } 
    }
}