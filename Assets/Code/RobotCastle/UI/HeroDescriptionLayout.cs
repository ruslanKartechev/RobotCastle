using UnityEngine;

namespace RobotCastle.UI
{
    public class HeroDescriptionLayout : MonoBehaviour
    {
        [SerializeField] private float _sizeYLong; 
        [SerializeField] private float _sizeYshort;
        [SerializeField] private RectTransform _parentRect;
        [SerializeField] private GameObject _spellBlock1;
        [SerializeField] private GameObject _spellBlock2;

        public void SetLong()
        {
            _spellBlock1.SetActive(true);
            _spellBlock2.SetActive(true);
            var size = _parentRect.sizeDelta;
            size.y = _sizeYLong;
            _parentRect.sizeDelta = size;

        }

        public void SetShort()
        {
            _spellBlock1.SetActive(false);
            _spellBlock2.SetActive(false);
            var size = _parentRect.sizeDelta;
            size.y = _sizeYshort;
            _parentRect.sizeDelta = size;
        }


    }
}