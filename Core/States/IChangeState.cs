namespace Core.States
{
    public interface IChangeState
    {
        void Next(FinitState finitState);
    }
}