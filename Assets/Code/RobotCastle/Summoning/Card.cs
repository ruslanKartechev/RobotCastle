using DG.Tweening;
using RobotCastle.Core;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Summoning
{
    public class Card : MonoBehaviour, IPoolItem
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _textCount;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _highlight;

        public void AnimateHighlight()
        {
            const float time = .344f;
            _highlight.DOKill();
            _highlight.gameObject.SetActive(true);
            _highlight.SetAlpha(0f);
            _highlight.DOFade(1f, time).OnComplete(() =>
            {
                _highlight.DOFade(0f, time).OnComplete(() =>
                {
                    _highlight.gameObject.SetActive(false);
                });
            });
        }

        public void StopHighlight()
        {
            _highlight.DOKill();
            _highlight.gameObject.SetActive(false);
        }
        
        public void SetTitleAndCount(string title, int count)
        {
            _title.text = title;
            _textCount.text = $"+{count}";
            _textCount.gameObject.SetActive(true);
        }
        
        public void SetTitleAndCount(string title, string countText)
        {
            _title.text = title;
            _textCount.text = countText;
            _textCount.gameObject.SetActive(true);
        }

        public void SetIcon(Sprite sprite)
        {
            _iconImage.sprite = sprite;
        }
        
        public void HideCountText()
        {
            _textCount.gameObject.SetActive(false);
        }

        public string PoolId { get; set; }

        public GameObject GetGameObject() => gameObject;
        
        public void PoolHide()
        {
            gameObject.SetActive(false);
        }

        public void PoolShow()
        {
            gameObject.SetActive(true);
        }
    }
}