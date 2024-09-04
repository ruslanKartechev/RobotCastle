using UnityEngine;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
namespace SleepDev
{
    public class ScaleEffect : ButtonClickEffect
    {
        [SerializeField] private Vector3 _punchScale;
        [SerializeField] private float _punchTime = .1f;
        
        
        public override void Play()
        {
            _target.localScale = Vector3.one;
#if HAS_DOTWEEN
            _target.DOPunchScale(_punchScale, _punchTime);
        #endif
        }
    }
}