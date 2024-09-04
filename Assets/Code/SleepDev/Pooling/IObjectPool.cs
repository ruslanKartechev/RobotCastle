namespace SleepDev
{
    public interface IObjectPool<T> where T : class
    {
        void BuildPool(int size);
        T GetObject();
        void ReturnObject(IPooledObject<T> obj);
        void ClearPool();
        T[] GetObjects(int count);
        int CurrentSize { get; }
    }
}