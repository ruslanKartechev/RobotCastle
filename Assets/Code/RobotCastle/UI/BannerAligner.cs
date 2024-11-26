using System;
using UnityEngine;

namespace RobotCastle.UI
{
    [DefaultExecutionOrder(500)]
    public class BannerAligner : MonoBehaviour
    {
        public RectTransform rect;
        public Vector2 normalSizeDelta;
        public Vector2 bannerSizeDelta;
        public bool startWithBanner;
        private bool _isBannerMode;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rect == null)
            {
                rect = gameObject.GetComponent<RectTransform>();
                normalSizeDelta = rect.sizeDelta;
                bannerSizeDelta = normalSizeDelta;
                bannerSizeDelta.y += 168;
                UnityEditor.EditorUtility.SetDirty(this);
            }

            if (rect != null)
            {
                // normalSizeDelta = rect.sizeDelta;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        private void Start()
        {
            if(startWithBanner)
                SetBanner();
        }

        private void OnEnable()
        {
         
        }

        public void SetBanner()
        {
            _isBannerMode = true;
            rect.sizeDelta = bannerSizeDelta;
        }

        public void SetNoBanner()
        {
            _isBannerMode = false;
            rect.sizeDelta = normalSizeDelta;
        }

        private void Update()
        {
            if(SleepDev.AdsPlayer.Instance == null)
                return;
            var shouldBeBanner = SleepDev.AdsPlayer.Instance.BannerCalled;
            if (_isBannerMode != shouldBeBanner)
            {
                if(shouldBeBanner)
                    SetBanner();
                else
                    SetNoBanner();
            }
        }
    }
}