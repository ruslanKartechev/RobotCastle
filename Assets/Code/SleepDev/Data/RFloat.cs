namespace SleepDev
{
    [System.Serializable]
    public class RFloat
    {
        public float min;
        public float max;

        public float Get
        {
            get
            {
                return UnityEngine.Random.Range(min, max);
            }
        }
    }
}