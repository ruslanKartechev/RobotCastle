#define SDK_MADPIXEL
#define SKIP_ALL_ADS
using System;
using System.Collections;
using MAXHelper;
using UnityEngine;

namespace SleepDev
{
    public class AdsPlayer : MonoBehaviour
    {
        public const string Placement_Merchant = "merchant_offer";
        public const string Placement_Shop = "shop_ads";
        public const string Placement_Double = "double_rewrad";

        
        private static AdsPlayer _instance;
        public static AdsPlayer Instance => _instance;

        public static void Create(float interTimerDelaySecs, AdPlayMode mode)
        {
            if (_instance != null)
                return;
            var go = new GameObject("go_AdsPlayer");
            _instance = go.AddComponent<AdsPlayer>();
            DontDestroyOnLoad(go);
            _instance.Init(interTimerDelaySecs, mode);
        }
        
        public enum AdPlayMode {Release, FakeAds, SkipAll}

        private AdPlayMode _mode;
        private float _interTimerDelaySecs = 30;
        private DateTime _lastInterTime;
        private Action<bool> _currentCallback;
        private bool _bannerCalled;

        public AdPlayMode AdMode
        {
            get => _mode;
            set => _mode = value;
        }
        
        public float InterTimerDelaySecs
        {
            get => _interTimerDelaySecs;
            set => _interTimerDelaySecs = value;
        }

        public bool BannerCalled => _bannerCalled;
        
        private AdsPlayer(){}

        private void ResetLastInterTime()
        {
            _lastInterTime = DateTime.Now - TimeSpan.FromDays(10);
        }
        
        private void Init(float interTimerDelaySecs, AdPlayMode mode)
        {
            _mode = mode;
            _interTimerDelaySecs = interTimerDelaySecs;
            ResetLastInterTime();
        }

        /// <summary>
        /// </summary>
        /// <returns>True if inter should be played. False if delay is not over</returns>
        public bool CheckInterTimeout()
        {
            var secondsSinceLast = (DateTime.Now - _lastInterTime).TotalSeconds;
            return secondsSinceLast > _interTimerDelaySecs;
        }
        
        public (bool, string) PlayInter(Action<bool> callback, string placement)
        {
            if (!CheckInterTimeout())
                return (false, "timer_not_ready");
            
            if (_mode == AdPlayMode.FakeAds)
            {
                FakeAdCountdownUI.Get().Show("Inter", callback);
                return (true, "skipped");
            }
#if SDK_MADPIXEL
            var outptuMsg = $"[{nameof(AdsPlayer)}] Show inter ad {placement}. ";
            var outputResult = false;
            var result = AdsManager.ShowInter(gameObject, OnInterClosed, placement);
            switch (result)
            {
                case AdsManager.EResultCode.OK:
                    _currentCallback = callback;
                    SoundOff();
                    outputResult = true;
                    _lastInterTime = DateTime.Now;
                    break;
                case AdsManager.EResultCode.ERROR:
                    outptuMsg += "EResultCode.Error";
                    break;
                case AdsManager.EResultCode.NOT_LOADED:
                    outptuMsg += "Inter not loaded";
                    break;
                case AdsManager.EResultCode.ON_COOLDOWN:
                    outptuMsg += "On Cooldown";
                    break;
            }
            return (outputResult, outptuMsg);
            #else
            callback?.Invoke(true);
            return (true, $"[{nameof(AdsPlayer)}] no_sdk.");
#endif
        }

        public (bool, string) PlayReward(Action<bool> callback, string placement)
        {
            if (_mode == AdPlayMode.FakeAds)
            {
                ResetLastInterTime();
                FakeAdCountdownUI.Get().Show("Reward", callback);
                return (true, "skipped");
            }
            
#if SDK_MADPIXEL
            var outptuMsg = $"[{nameof(AdsPlayer)}] Show rewarded {placement}. ";
            var outputResult = false;
            var result = AdsManager.ShowRewarded(gameObject, OnRewardedClosed, placement);
            switch (result)
            {
                case AdsManager.EResultCode.OK:
                    _currentCallback = callback;
                    SoundOff();
                    outputResult = true;
                    ResetLastInterTime();
                    break;
                case AdsManager.EResultCode.ERROR:
                    outptuMsg += "EResultCode.Error";
                    break;
                case AdsManager.EResultCode.NOT_LOADED:
                    outptuMsg += "Rewarded not loaded";
                    break;
                case AdsManager.EResultCode.ON_COOLDOWN:
                    outptuMsg += "On Cooldown";
                    break;
            }
            return (outputResult, outptuMsg);
#else
            callback?.Invoke(true);
            return (true, $"[{nameof(AdsPlayer)}] no_sdk.");
#endif
        }

        public void ShowBanner()
        {
#if SDK_MADPIXEL
            CLog.Log($"[{nameof(AdsPlayer)}] ShowBanner");
            _bannerCalled = true;
            MAXHelper.AdsManager.ToggleBanner(_bannerCalled);
#endif
        }

        public void HideBanner()
        {
#if SDK_MADPIXEL
            CLog.Log($"[{nameof(AdsPlayer)}] HideBanner");
            _bannerCalled = false;
            MAXHelper.AdsManager.ToggleBanner(_bannerCalled);
#endif
        }

        private void OnRewardedClosed(bool result)
        {
            CLog.LogGreen($"[{nameof(AdsPlayer)}] OnRewardedShown. Result: {result}");
            SoundOn();
            _currentCallback?.Invoke(result);
            _currentCallback = null;
        }
        
        private void OnInterClosed(bool result)
        {
            CLog.LogGreen($"[{nameof(AdsPlayer)}] OnInterClosed. Result: {result}");
            SoundOn();
            _currentCallback?.Invoke(result);
            _currentCallback = null;
        }
        
        private void SoundOn()
        {
            // CLog.Log($"[{nameof(AdsPlayer)}] SoundOn");
            // if( SoundContainer.MusicPlayer != null)
            //     SoundContainer.MusicPlayer.Mute(false);
        }

        private void SoundOff()
        {
            // CLog.Log($"[{nameof(AdsPlayer)}] SoundOff");
            // if( SoundContainer.MusicPlayer != null)
            //     SoundContainer.MusicPlayer.Mute(true);
        }

    }
}