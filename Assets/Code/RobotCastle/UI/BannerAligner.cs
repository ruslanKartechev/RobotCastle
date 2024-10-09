﻿using UnityEngine;

namespace RobotCastle.UI
{
    public class BannerAligner : MonoBehaviour
    {
        public RectTransform rect;
        public Vector2 normalSizeDelta;
        public Vector2 bannerSizeDelta;
        public bool startWithBanner;

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
                normalSizeDelta = rect.sizeDelta;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        private void Start()
        {
            if(startWithBanner)
                SetBanner();
        }

        public void SetBanner()
        {
            rect.sizeDelta = bannerSizeDelta;
        }

        public void SetNoBanner()
        {
            rect.sizeDelta = normalSizeDelta;
        }
    }
}