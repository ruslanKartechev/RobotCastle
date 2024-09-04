using UnityEngine;

namespace SleepDev.Chunks
{
    [CreateAssetMenu(menuName = "SO/Chunk Manager Settings", fileName = "Chunk Manager Settings", order = -200)]
    public class ChunkManagerSettings : ScriptableObject
    { 
        public float defaultRadius;

    }
}