using DG.Tweening;
using RobotCastle.Core;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class DamageUI : MonoBehaviour, IPoolItem
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;

        public Pool pool { get; set; }

        public void Show(int amount)
        {
            _text.text = $"{amount}";
            gameObject.SetActive(true);
        }

        public void AnimateDamage(Ease ease)
        {
            _canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one * 1.1f;
            transform.DOScale(0.85f, .325f);
            _canvasGroup.DOFade(0f, .82f).OnComplete(Return);
        }

        public void ShowMightyBlock()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            transform.DOPunchScale(Vector3.one * .135f, .42f);
            _canvasGroup.DOFade(0f, .5f).SetDelay(.5f).OnComplete(Return);
        }

        public void ShowVampirism(int amount)
        {
            _text.text = $"+{amount}";
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;
            transform.DOScale(0.85f, .6f);
            var pos = transform.position;
            var y = pos.y + 15 * (Screen.height / 1980);

            transform.DOMoveY(y, .72f);
            _canvasGroup.DOFade(0f, .32f).SetDelay(.4f).OnComplete(Return);
        }

        public GameObject GetGameObject() => gameObject;

        public string PoolId { get; set; }
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(true);

        private void Off() => gameObject.SetActive(false);

        private void Return()
        {
            gameObject.SetActive(true);
            pool.Return(this);
        }
    }
}