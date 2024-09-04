using UnityEngine;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/Sound/RandomVolumeSound", fileName = "RandomVolumeSound", order = 0)]
    public class RandomVolumeSoundSo : SoundSo
    {
        [SerializeField] private Vector2 _volumeLimits;

        public override PlayingSound Play()
        {
            return SoundContainer.SoundManager.Play(this, false, _volumeLimits.Random());
        }
    }
}