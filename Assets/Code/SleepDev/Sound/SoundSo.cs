using UnityEngine;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/Sound/Sound", fileName = "Sound", order = 0)]
    public class SoundSo : SoundID
    {
        [SerializeField] protected bool _loop;
        
        public virtual PlayingSound Play()
        {
            return SoundContainer.SoundManager.Play(this, _loop);
        }
    }
}