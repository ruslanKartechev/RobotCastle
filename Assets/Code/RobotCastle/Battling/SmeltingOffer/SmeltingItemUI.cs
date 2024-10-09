using DG.Tweening;
using SleepDev.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Battling.SmeltingOffer
{
    public class SmeltingItemUI : ScaleItem
    {
        public Image icon => _icon;

        public void AnimateShow()
        {
            const float time = .55f;
            var angles = transform.eulerAngles;
            angles.y = 90f;
            transform.eulerAngles = angles;
            transform.DORotate(Vector3.zero, time).SetEase(Ease.OutBack);
        }
    }
}