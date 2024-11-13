namespace SleepDev
{
    [System.Serializable]
    public class RInt
    {
        public int min;
        public int max;

        public int Val => UnityEngine.Random.Range(min, max+1);
    }
}