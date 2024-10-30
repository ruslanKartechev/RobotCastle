using UnityEngine;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/Sound/RandomVolumePitchSound", fileName = "RandomVolumePitchSound", order = 0)]
    public class RandomVolumePitchSoundSo : SoundSo
    {
        [SerializeField] private Vector2 _volumeLimits;
        [SerializeField] private Vector2 _pitchLimits;

        public override PlayingSound Play()
        {
            return SoundContainer.SoundManager.Play(this, false, volume * _volumeLimits.Random(), _pitchLimits.Random());
        }
    }
}