using UnityEngine;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/" + nameof(SlowMotionEffectSO), fileName = nameof(SlowMotionEffectSO), order = 0)]
    public class SlowMotionEffectSO : ScriptableObject
    {
        [SerializeField] private SlowMotionEffect _effect;
        
        public SlowMotionEffect Effect => _effect;

        public void Begin()
        {
            SlowMotionManager.Inst.Begin(Effect);
        }

        public void Stop()
        {
            SlowMotionManager.Inst.Exit(Effect);
        }
    }
}