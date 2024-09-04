using UnityEngine;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
namespace SleepDev
{
    public class TutorialHand : MonoBehaviour
    {
        public RectTransform movable;
        public float moveSpeed;
        [Space(10)]
        public float downScale;
        public float clickTime;
        public float upTime;
        // [Space(10)]
        private Coroutine _moving;
#if HAS_DOTWEEN
        private Sequence _sequence;
#endif
        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }
        
        public void WarpTo(Vector3 position)
        {
            StopMoving();
            movable.position = position;
        }

        public void StopMoving()
        {
            // movable.DOKill();
#if HAS_DOTWEEN
            _sequence.Kill();
#endif
}

        public void MoveTo(Vector3 position, float time)
        {
            StopMoving();
#if HAS_DOTWEEN
            movable.DOMove(position, time);
#endif
}
        
        public void MoveTo(Vector3 position)
        {
            var time = (position - movable.position).magnitude / moveSpeed;
            StopMoving();
#if HAS_DOTWEEN
            movable.DOMove(position, time);
#endif
}
        
        public void MoveBetween(Vector3 pos1, Vector3 pos2, float time)
        {
            // Debug.Log($"moving between {pos1} and {pos2}, time {time}");
            WarpTo(pos1);
            movable.localScale = Vector3.one;
#if HAS_DOTWEEN
            _sequence = DOTween.Sequence();
            _sequence.Append(movable.DOMove(pos2, time).SetEase(Ease.Linear));
            _sequence.Append(movable.DOMove(pos1, time).SetEase(Ease.Linear));
            _sequence.SetLoops(-1);
#endif
        }
        
        public void MoveBetween(Vector3 pos1, Vector3 pos2, float timeTo2, float timeTo1, float delay)
        {
            // Debug.Log($"moving between {pos1} and {pos2}, time {time}");
            WarpTo(pos1);
            movable.localScale = Vector3.one;
#if HAS_DOTWEEN
            _sequence = DOTween.Sequence();
            _sequence.Append(movable.DOMove(pos2, timeTo2).SetEase(Ease.Linear));
            _sequence.Append(movable.DOMove(pos1, timeTo1).SetEase(Ease.Linear).SetDelay(delay));
            _sequence.SetLoops(-1);
#endif
        }
        
        public void LoopClicking(Vector3 pos)
        {
            StopMoving();
            WarpTo(pos);
#if HAS_DOTWEEN
            _sequence = DOTween.Sequence();
            _sequence.Append(movable.DOScale(Vector3.one * downScale, clickTime).SetEase(Ease.Linear));
            _sequence.Append(movable.DOScale(Vector3.one, upTime));
            _sequence.SetLoops(-1);
#endif
        }

    }
}