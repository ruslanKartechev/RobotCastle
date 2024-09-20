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

        public void Animate(float time, Ease ease)
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            _canvasGroup.DOFade(0f, time).SetEase(ease).OnComplete(Off);
        }

        private void Off() => gameObject.SetActive(false);
        
        
    }
}