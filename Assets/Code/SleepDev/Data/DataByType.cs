namespace SleepDev
{
    [System.Serializable]
    public class DataByType<TData, TType>
    {
        public TData data;
        public TType type;
    }
}