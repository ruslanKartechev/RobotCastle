#define HAS_SDK
#define LOG_IN_BUILD
using System.Collections.Generic;

namespace SleepDev
{
    public static class Analytics
    {
        
        public static void OnLevelStarted(int chapter, int tier)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== [Analytics] Event Start chapter: {chapter+1}, tier: {tier+1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_start", new Dictionary<string, object>()
            {
                {"chapter", chapter+1},
                {"difficulty", tier+1},
            });
            // AppMetrica.Instance?.SendEventsBuffer();
#endif
        }

        public static void LevelReplayCalled(int chapter, int tier)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== [Analytics] Event Replay chapter: {chapter+1}, tier: {tier+1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"chapter", chapter+1},
                {"difficulty", tier+1},
            });
#endif
        }
        
        public static void OnLevelCompleted(int chapter, int tier)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== [Analytics] Event Start chapter: {chapter+1}, tier: {tier+1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"chapter", chapter+1},
                {"difficulty", tier+1},
                {"result", "win" }
            });
#endif
        }
        
        public static void LevelFailed(int chapter, int tier)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== [Analytics] Event Start chapter: {chapter+1}, tier: {tier+1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"chapter", chapter+1},
                {"difficulty", tier+1},
                {"result", "failed" }
            });
#endif
        }

        public static void HeroUpgraded(string heroId, int newHeroLevel)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== [Analytics] Hero Upgrade: {heroId}, new level {newHeroLevel+1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"hero_id", heroId},
                {"hero_level", newHeroLevel + 1}
            });
#endif
        }
        
        
        
        public static void OnTutorialCompleted(string name)
        {
#if HAS_SDK
            MadPixelAnalytics.AnalyticsManager.CustomEvent("tutorial_completed", new Dictionary<string, object>()
            {
                {"tutorial_name", name}
            });
#endif
        }

        
        public static void OnReward(int index)
        {
#if HAS_SDK
            const string eventName = "ad_reward";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName} {index+1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>()
            {
                {"level_number", index + 1},
            });
#endif
        }

        public static void OnInter(int index)
        {
#if HAS_SDK
            const string eventName = "ad_inter";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName} {index+1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>()
            {
                {"level_number", index + 1},
            });
#endif
        }
        

        public static void OnMergeNewItem(int itemLevel)
        {
#if HAS_SDK
            const string eventName = "item_merged";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName} {itemLevel}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>()
            {
                {"level", itemLevel},
            });
#endif
        }
        

    }
}