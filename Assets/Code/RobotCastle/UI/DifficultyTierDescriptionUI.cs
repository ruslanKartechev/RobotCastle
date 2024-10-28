using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class DifficultyTierDescriptionUI : MonoBehaviour
    {
        [SerializeField] private float _fadeTime = .25f;
        [SerializeField] private float _startAlpha = .25f;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _group;
        [Space(5)]
        [SerializeField] private float _startHeight;
        [SerializeField] private float _heightPerLine;
        [SerializeField] private float _nextLineDelay = .1f;
        [SerializeField] private RectTransform _rect;
        private Coroutine _working;

        public void Show(string description, bool animated = true)
        {
            description = description.Replace("<br>", "");
            var lines = description.Split('\n');
            var linesCount = lines.Length;
            var size = _rect.sizeDelta;
            size.y = _startHeight + _heightPerLine * (linesCount - 1);
            _rect.sizeDelta = size;

            _text.text = description;
            gameObject.SetActive(true);
            if (animated)
            {
                _group.alpha = _startAlpha;
                _group.DOFade(1f, _fadeTime);
            }
            else
                _group.alpha = 1f;
            // if(_working != null)
                // StopCoroutine(_working);
            // _working = StartCoroutine(Working(lines));
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
            // if(_working != null)
                // StopCoroutine(_working);
        }

        private IEnumerator Working(string[] lines)
        {
            var count = lines.Length;
            var txt = "";
            for (var i = 0; i < count; i++)
            {
                txt += lines[i];
                _text.text = txt;
                yield return new WaitForSeconds(_nextLineDelay);
            }
        }
    }
}