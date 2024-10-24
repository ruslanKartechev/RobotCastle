using RobotCastle.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.DevCheat
{
    public class ButtonWithCheck : MyButton
    {

        public void SetState(bool state)
        {
            _checkMark.gameObject.SetActive(state);
            if(_btnImage != null && _changeSprite)
            {
                _btnImage.sprite = state ? _iconActive : _iconPassive;
            }
        }

        [SerializeField] private Image _checkMark;
        [SerializeField] private bool _changeSprite;
        [SerializeField] private Image _btnImage;
        [SerializeField] private Sprite _iconActive;
        [SerializeField] private Sprite _iconPassive;
    }
}