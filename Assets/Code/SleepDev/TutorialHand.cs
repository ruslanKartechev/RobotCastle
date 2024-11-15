using System.Collections;
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
        private Sequence _seqScaling;
        private Sequence _seqMoving;
#endif
        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            StopAllActions();
            gameObject.SetActive(false);
        }
        
        public void WarpTo(Vector3 position)
        {
            _seqMoving?.Kill();
            movable.position = position;
        }

        public void StopAllActions()
        {
            if(_moving != null)
                StopCoroutine(_moving);
#if HAS_DOTWEEN
            _seqScaling?.Kill();
            _seqMoving?.Kill();
#endif
        }
        
        public void StopMoving()
        {
#if HAS_DOTWEEN
            _seqMoving?.Kill();
#endif
        }

        public void StopScaling()
        {
#if HAS_DOTWEEN
            _seqScaling?.Kill();
#endif
        }

        public void MoveTo(Vector3 position, float time)
        {
            StopAllActions();
#if HAS_DOTWEEN
            _seqMoving?.Kill();
            _seqMoving = DOTween.Sequence();
            _seqMoving.Append(movable.DOMove(position, time));
#endif
        }
        
        public void MoveTo(Vector3 position)
        {
            var time = (position - movable.position).magnitude / moveSpeed;
            StopAllActions();
#if HAS_DOTWEEN
            _seqMoving?.Kill();
            _seqMoving = DOTween.Sequence();
            _seqMoving.Append(movable.DOMove(position, time).SetEase(Ease.OutCubic));
#endif
        }
        
        public void MoveToAndLoopClicking(Vector3 position)
        {
            var time = (position - movable.position).magnitude / moveSpeed;
            StopAllActions();
#if HAS_DOTWEEN
            _seqMoving?.Kill();
            _seqMoving = DOTween.Sequence();
            _seqMoving.Append(movable.DOMove(position, time).SetEase(Ease.OutCubic));
            _seqMoving.OnComplete(BeginClickingLoop);
#endif
        }
        
        public void MoveBetween(Vector3 pos1, Vector3 pos2, float time)
        {
            // Debug.Log($"moving between {pos1} and {pos2}, time {time}");
            WarpTo(pos1);
            movable.localScale = Vector3.one;
#if HAS_DOTWEEN
            _seqMoving?.Kill();
            _seqMoving = DOTween.Sequence();
            _seqMoving.Append(movable.DOMove(pos2, time).SetEase(Ease.OutCubic));
            _seqMoving.Append(movable.DOMove(pos1, time).SetEase(Ease.OutCubic));
            _seqMoving.SetLoops(-1);
#endif
        }
        
        public void MoveBetween(Vector3 pos1, Vector3 pos2, float timeTo2, float timeTo1, float delay)
        {
            if(_moving != null)
                StopCoroutine(_moving);
            StopMoving();
            movable.position = pos1;
            movable.localScale = Vector3.one;
#if HAS_DOTWEEN
            _seqMoving?.Kill();
            _seqMoving = DOTween.Sequence();
            _seqMoving.Append(movable.DOMove(pos2, timeTo2).SetEase(Ease.OutCubic));
            _seqMoving.Append(movable.DOMove(pos1, timeTo1).SetEase(Ease.OutCubic).SetDelay(delay));
            _seqMoving.SetLoops(-1);
#endif
        }
        
        public void LoopClicking(Vector3 pos)
        {
            StopAllActions();
            WarpTo(pos);
#if HAS_DOTWEEN
            BeginClickingLoop();
#endif
        }
        
        public void LoopClickingTracking(Transform point, Vector3 offset, float moveToPointTime = 1f)
        {
            StopAllActions();
            _moving = StartCoroutine(Tracking(point, offset, moveToPointTime));
        }

        private IEnumerator Tracking(Transform point, Vector3 offset, float moveTime)
        {
            var p1 = movable.position;
            var elapsed = 0f;
            while (elapsed < moveTime)
            {
                movable.position = Vector3.Lerp(p1, point.position + offset, elapsed / moveTime);
                elapsed += Time.deltaTime;
            }
#if HAS_DOTWEEN
            BeginClickingLoop();
#endif
            while (true)
            {
                movable.position = point.position + offset;
                yield return null;
            }
        }

        private void BeginClickingLoop()
        {
            _seqScaling?.Kill();
            _seqScaling = DOTween.Sequence();
            movable.localScale = Vector3.one;
            _seqScaling.Append(movable.DOScale(Vector3.one * downScale, clickTime).SetEase(Ease.OutCubic));
            _seqScaling.Append(movable.DOScale(Vector3.one, upTime).SetEase(Ease.OutCubic));
            _seqScaling.SetLoops(-1);
        }

    }
}