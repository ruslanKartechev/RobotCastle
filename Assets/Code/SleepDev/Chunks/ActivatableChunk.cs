using UnityEngine;

namespace SleepDev.Chunks
{
    public class ActivatableChunk : BaseChunk
    {
        [SerializeField] private float _size = 4;
        private bool _isLoaded;
        public override float Size => _size;

        public override void Load()
        {
            if (_isLoaded) return;
            _isLoaded = true;
        }

        public override void Unload()
        {
            if (!_isLoaded) return;
            _isLoaded = false;
        }
    }
}