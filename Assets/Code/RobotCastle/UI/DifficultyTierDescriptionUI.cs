using DG.Tweening;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class DifficultyTierDescriptionUI : MonoBehaviour
    {
        [SerializeField] private float _fadeTime = .25f;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _group;
        [Space(5)]
        [SerializeField] private float _startHeight;
        [SerializeField] private float _heightPerLine;
        [SerializeField] private RectTransform _rect;
        
        public void Show(string description, bool animated = true) 
        {
            var lines = description.Split('\n').Length;
            var size = _rect.sizeDelta;
            size.y = _startHeight + _heightPerLine * (lines - 1);
            _rect.sizeDelta = size;

            _text.text = description;
            gameObject.SetActive(true);
            if (animated)
            {
                _group.alpha = 0f;
                _group.DOFade(1f, _fadeTime);
            }
            else
            {
                _group.alpha = 1f;
            }
        }

        public void Hide(bool animated = false)
        {
            if (animated)
            {
                _group.alpha = 1f;
                _group.DOFade(0f, _fadeTime);
            }
            else
            {
                _group.alpha = 0f;
                gameObject.SetActive(false);
            }
        }
    }
}