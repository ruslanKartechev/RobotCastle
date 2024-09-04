using UnityEngine;

namespace SleepDev.Chunks
{
    public interface IChunkManager
    {
        Camera ActiveCamera { get; set; }
        float Radius { get; set; } 
        void Activate();
        void Deactivate();
    }
}