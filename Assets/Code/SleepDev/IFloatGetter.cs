namespace SleepDev
{
    public interface IFloatGetter
    {
        float Get();
    }

    public interface IValueGetter<T>
    {
        T Get();
    }
}