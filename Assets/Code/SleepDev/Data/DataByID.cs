namespace SleepDev
{
    [System.Serializable]
    public class DataByID<T>
    {
        public string id;
        public T obj;
        
        public DataByID()
        {
        }
        
        public DataByID(T obj, string id)
        {
            this.obj = obj;
            this.id = id;
        }
    }
}