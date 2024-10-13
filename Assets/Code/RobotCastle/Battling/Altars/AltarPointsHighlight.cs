using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Battling.Altars
{
    public class AltarPointsHighlight : MonoBehaviour
    {
        [SerializeField] private Image _highlightImage;

        public void SetState(bool on)
        {
            _highlightImage.gameObject.SetActive(on);
        }

        public void AnimateOn()
        {
            _highlightImage.gameObject.SetActive(true);
            _highlightImage.transform.localScale = Vector3.one;
            _highlightImage.transform.DOPunchScale(Vector3.one * .2f, .2f);
        }

    }
}