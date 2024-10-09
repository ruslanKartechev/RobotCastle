using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SleepDev.Inventory
{
    public class FadeHighlightItem : BasicItem
    {
        [SerializeField] private Image _fadeImage;
        public const float FadeTime = .25f;
        
        
        public override void Pick()
        {
            base.Pick();
            _fadeImage.gameObject.SetActive(true);
            _fadeImage.SetAlpha(0f);
            _fadeImage.DOFade(1f, FadeTime);
        }

        public override void Unpick()
        {
            base.Unpick();
            _fadeImage.DOFade(0f, FadeTime);
        }
    }
}