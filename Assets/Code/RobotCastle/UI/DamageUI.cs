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
            _canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one * 1.1f;
            transform.DOScale(0.85f, .325f);
            _canvasGroup.DOFade(0f, .82f).OnComplete(Off);
        }

        private void Off() => gameObject.SetActive(false);
        
        
    }
}