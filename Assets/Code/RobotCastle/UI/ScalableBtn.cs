using DG.Tweening;
using SleepDev;
using UnityEngine.EventSystems;

namespace RobotCastle.UI
{
    public class ScalableBtn : ProperButton 
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            transform.DOScale(.8f, .25f);
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            transform.DOScale(1f, .25f);
        }
        
    }
}