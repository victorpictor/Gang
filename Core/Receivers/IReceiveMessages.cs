using Core.Messages;
using Core.States.Services;

namespace Core.Receivers
{
    public interface IReceiveMessages
    {
        IMessage Receive();
    }

   
    public interface IReceiveMessagesService : IReceiveMessages
    {
        void StartService();
        void StopService();
    }
   
}