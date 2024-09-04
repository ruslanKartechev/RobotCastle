using UnityEngine;

namespace SleepDev
{
    public class AnimationDurationPrinter : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private int _targetLayer = 0;
        
        public void PrintAll()
        {
            
            var clips = _animator.runtimeAnimatorController.animationClips;
            // _animator.GetNextAnimatorClipInfo()
            if (clips == null)
            {
                CLog.Log($"Animator clips info for layer {_targetLayer} is null");
                return;
            }
            if (clips.Length == 0)
            {
                CLog.Log($"Animator clips info length == 0");
                return;
            }

            foreach (var clip in clips)
            {
                CLog.Log($"Name: {clip.name}, Length {clip.length} s");
            }
        }
    }
}