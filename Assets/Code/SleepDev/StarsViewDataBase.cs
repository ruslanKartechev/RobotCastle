using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/StarsViewDataBase", fileName = "StarsViewDataBase", order = 10)]
    public class StarsViewDataBase : ScriptableObject
    {
        public List<int> levelsThresholds;
        public List<Sprite> images;
    }
}