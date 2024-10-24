using System.Collections.Generic;

namespace SleepDev
{
    [System.Serializable]
    public class WeightedList<T>
    {
        public List<Data<T>> options;
        private float _total;

        public float Total => _total;

        public void CalculateTotal()
        {
            var total = 0f;
            foreach (var t in options)
                total += t.weight;
            _total = total;
        }

        public T Random(bool recalculateTotal = true)
        {
            if (options.Count == 0)
                return default;
            if (options.Count == 1)
                return options[0].obj;
            if(recalculateTotal || _total == 0)
                CalculateTotal();
            var random = UnityEngine.Random.Range(0f, _total);
            var val = 0f;
            for (var i = 0; i < options.Count; i++)
            {
                val += options[i].weight;
                if (val >= random)
                {
                    return options[i].obj;
                }
            }
            throw new System.Exception("Gone through all options... possible error");
        }

        [System.Serializable]
        public class Data<T>
        {
            public T obj;
            public float weight;
            
            public Data(){}

            public Data(T obj, float weight)
            {
                this.obj = obj;
                this.weight = weight;
            }
        }
    }
    
}