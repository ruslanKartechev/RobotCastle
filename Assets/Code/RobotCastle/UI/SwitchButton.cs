using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class SwitchButton : MyButton
    {
        [SerializeField] private Vector2 _positions;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _activeImage;
        [SerializeField] private RectTransform _rect;
        private const float MoveTime = .22f;    
        
        public void SetActive(bool animate)
        {
            _activeImage.gameObject.SetActive(true);
            _text.text = "On";
            var pos = _rect.anchoredPosition;
            pos.x = _positions.x;
            if (animate)
            {
                _rect.DOAnchorPos(pos, MoveTime);
            }
            else
            {
             
                _rect.anchoredPosition = pos;
            }
        }

        public void SetPassive(bool animate)
        {
            _activeImage.gameObject.SetActive(false);
            _text.text = "Off";
            var pos = _rect.anchoredPosition;
            pos.x = _positions.y;
            if (animate)
            {
                _rect.DOAnchorPos(pos, MoveTime);
            }
            else
            {
                _rect.anchoredPosition = pos;
            }
        }

    }
}