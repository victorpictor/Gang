using System.Threading;
using Core.States.Services;

namespace Core.States.TheLead
{
    public class ReadLogService : AbstractService
    {
        public ReadLogService()
        {
            reference = new ServiceReference(()=> Thread.Sleep(3000));
        }
    }
}