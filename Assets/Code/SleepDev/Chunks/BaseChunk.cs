using UnityEngine;

namespace SleepDev.Chunks
{
    public abstract class BaseChunk : MonoBehaviour
    {
        public abstract float Size { get; }
        public abstract void Load();
        public abstract void Unload();
    }

}