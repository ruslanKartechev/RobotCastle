namespace SleepDev
{
    [System.Serializable]
    public class RInt
    {
        public int min;
        public int max;

        public float Val
        {
            get
            {
                return UnityEngine.Random.Range(min, max+1);
            }
        }
    }
}