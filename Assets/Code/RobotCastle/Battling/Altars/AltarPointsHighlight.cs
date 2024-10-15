using DG.Tweening;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Battling.Altars
{
    public class AltarPointsHighlight : MonoBehaviour
    {
        [SerializeField] private Color _passiveColor = Color.gray;
        [SerializeField] private Color _activeColor = Color.blue;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private FadeEffect _fadeEffect;

        public void SetState(bool on)
        {
            // _highlightImage.DOKill();
            _highlightImage.color = on ? _activeColor : _passiveColor;
        }

        public void AnimateOn()
        {
            _highlightImage.DOKill();
            _highlightImage.transform.localScale = Vector3.one;
            _highlightImage.transform.DOPunchScale(Vector3.one * .3f, .3f);
            _highlightImage.DOColor(_activeColor, .2f);
            if(_fadeEffect != null)
                _fadeEffect.Play();
        }
        


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_highlightImage == null)
            {
                _highlightImage = gameObject.GetComponent<Image>();
                if(_highlightImage != null)
                    UnityEditor.EditorUtility.SetDirty(this);
                else
                    CLog.Log($"{gameObject.name} Image not found!");
            }
            
        }
#endif

    }
}