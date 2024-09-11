using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class TestDescriptionUI : DescriptionUI
    {
        [SerializeField] private FadeInOutAnimator _animator;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _text;
        
        public override void Show(GameObject source)
        {
            var src = source.GetComponent<TestDescriptionProvider>();
            _text.text = src.GetText();
            _iconImage.sprite = src.GetIcon();
            transform.position = Camera.main.WorldToScreenPoint(src.WorldPosition);
            _animator.FadeIn();
        }

        public override void Hide()
        {
            _animator.FadeOut();
        }
    }
}