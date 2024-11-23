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
            CLog.Log($"=== [Analytics] Event Start chapter: {chapter + 1}, tier: {tier + 1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_start", new Dictionary<string, object>()
            {
                {"chapter", chapter + 1},
                {"difficulty", tier + 1},
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
                {"chapter", chapter + 1},
                {"difficulty", tier + 1},
            });
#endif
        }
        
        public static void OnLevelCompleted(int chapter, int tier, int round, string mode)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"[Analytics] [OnLevelCompleted]: Chapter {chapter+1}, tier: {tier+1}, round: {round}, mode: {mode}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"chapter", chapter + 1},
                {"difficulty", tier + 1},
                {"round", round },
                {"mode", mode },
                {"result", "win" }
            });
#endif
        }
        
        public static void OnLevelFailed(int chapter, int tier, int round, string mode)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"[Analytics] [OnLevelFailed]: Chapter {chapter+1}, tier: {tier+1}, round: {round}, mode: {mode}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"chapter", chapter + 1 },
                {"difficulty", tier + 1 },
                {"round", round },
                {"mode", mode },
                {"result", "failed" }
            });
#endif
        }

        public static void HeroPurchased(string heroId, int chapter, int tier, int round)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            // CLog.Log($"[Analytics] [OnLevelFailed]: Chapter {chapter+1}, tier: {tier+1}, round: {round}, mode: {mode}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("merge_hero_purchase", new Dictionary<string, object>()
            {
                {"hero_id", heroId },
                {"chapter", chapter + 1 },
                {"difficulty", tier + 1 },
                {"round", round },
                {"result", "failed" }
            });
#endif
        }        
        
        public static void HeroNewLevelUpgrade(string heroId, int newLevel)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"[Analytics] Hero Upgrade: {heroId}, new level {newLevel+1}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("hero_upgraded", new Dictionary<string, object>()
            {
                {"hero_id", heroId},
                {"hero_level", newLevel}
            });
#endif
        }
        
        public static void OnAltarPointAdded(string altar, int totalPoints)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"[Analytics] OnAltarPointAdded: {altar}, {totalPoints}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("altar_point_added", new Dictionary<string, object>()
            {
                {"altar", altar},
                {"total_points_amount", totalPoints}
            });
#endif
        }
        
        public static void AltarPointsReset(int totalPlayerPoints)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"[Analytics] AltarPointsReset: {totalPlayerPoints}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("altar_points_reset", new Dictionary<string, object>()
            {
                {"total_player_points", totalPlayerPoints}
            });
#endif
        }
        
        public static void AltarPointsPurchased(int totalPlayerPoints)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"[Analytics] AltarPointsPurchased: {totalPlayerPoints}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("altar_point_purchased", new Dictionary<string, object>()
            {
                {"total_player_points", totalPlayerPoints}
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
        
        public static void OnSummonUsed(string summonId)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"[Analytics] OnSummonUsed: {summonId}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("summon_scroll_used", new Dictionary<string, object>()
            {
                {"scroll_id", summonId}
            });
#endif
        }

        
        public static void OnNewHeroObtained(string heroId)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"[Analytics] OnNewHeroObtained: {heroId}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("new_hero_obtained", new Dictionary<string, object>()
            {
                {"hero_id", heroId}
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