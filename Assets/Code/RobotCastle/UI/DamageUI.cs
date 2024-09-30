using DG.Tweening;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class DamageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;

        public void Show(int amount)
        {
            _text. text = $"{amount}";
            gameObject.SetActive(true);
        }

        public void Animate(Ease ease)
        {
            const float punch = .125f;
            const float punchTime = .25f;
            const float fadeTime = .3f;
            // const float upPos = 50f;
            _canvasGroup.alpha = 1f;
            transform.DOPunchScale(new Vector3(punch, punch, punch), punchTime).OnComplete(() =>
            {
                // var pos = transform.position;
                // pos.y += upPos;
                // transform.DOMove(pos, fadeTime);
                _canvasGroup.DOFade(0f, fadeTime).SetEase(ease).OnComplete(Off);
            });
        }

        private void Off() => gameObject.SetActive(false);
        
        
    }
}