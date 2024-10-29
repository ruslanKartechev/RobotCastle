using System.Collections.Generic;
using UnityEngine;

namespace Bomber
{
    [CreateAssetMenu(menuName = "SO/ObstaclesContainer", fileName = "ObstaclesContainer", order = 0)]
    public class ObstaclesContainer : ScriptableObject
    {
        public List<string> prefabsOptions;
        public List<ObstaclesPreset> options;
        
    }
}