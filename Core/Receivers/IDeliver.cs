namespace Core.Receivers
{
    public interface IDeliver<T>
    {
        void Deliver(T message);
    }

    public interface IDeliverWith<T>
    {
        void Deliver(T deliverer);
    }
}