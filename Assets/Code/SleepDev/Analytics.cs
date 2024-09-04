#define HAS_SDK
#define LOG_IN_BUILD
using System.Collections.Generic;

namespace SleepDev
{
    public static class Analytics
    {
        public static int MissCount { get; set; }
        
        public static void OnStarted(int index, string levelMode)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== [Analytics] Event Start Level: {index+1}. Mode: {levelMode}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_start", new Dictionary<string, object>()
            {
                {"level_number", index+1},
                {"level_type", levelMode},
            });
            // AppMetrica.Instance?.SendEventsBuffer();
#endif
        }
        
        public static void OnWin(int index, string levelMode, float playTime, int missCount, int attemptsCount)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== [Analytics] Event_Win Level: {index+1}. PlayTime: {playTime}. Mode: {levelMode}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"level_number", index + 1},
                {"level_attempts_count", attemptsCount},
                {"level_type", levelMode},
                {"result", "win"},
                {"time", playTime},
                {"miss_count", missCount}
            });
#endif
        }
        
        public static void OnFailed(int index, string levelMode, float playTime, int missCount, int attemptsCount)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== [Analytics] Event_Failed Level: {index+1}. Time: {playTime}. Mode: {levelMode}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"level_number", index + 1},
                {"level_attempts_count", attemptsCount},
                {"level_type", levelMode},
                {"result", "loose"},
                {"time", playTime},
                {"miss_count", missCount}

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

        
        public static void OnCellPurchased(int x, int y)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] On Cell Purchased: {x}, {y}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent($"cell_purchased", 
            new Dictionary<string, object>()
            {
                {"cell_position", $"{x}, {y}"}
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


        
        public static void OnTalentAdded(string talentName)
        {
#if HAS_SDK
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] talent {talentName}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("talent_added", new Dictionary<string, object>()
            {
                {"talent_name", talentName},
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
        
        public static void OnQuestCompleted(string id)
        {
#if HAS_SDK
            const string eventName = "quest_completed";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName} {id}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>()
            {
                {"quest_id", id},
            });
#endif
        }
        
        public static void OnEggOpened(int id)
        {
#if HAS_SDK
            const string eventName = "egg_opened";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName} {id}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>()
            {
                {"egg_id", id},
            });
#endif
        }
        
        public static void OnEggStarted(int id)
        {
#if HAS_SDK
            const string eventName = "egg_started";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName} {id}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>()
            {
                {"egg_id", id},
            });
#endif
        }

        public static void OnSpinWheelOpened()
        {
#if HAS_SDK
            const string eventName = "spin_wheel_opened";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>());
#endif
        }

        public static void OnSpinWheelPlayed()
        {
#if HAS_SDK
            const string eventName = "spin_wheel_played";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>());
#endif
        }

        public static void OnAutoHunt(int levelIndex)
        {
#if HAS_SDK
            const string eventName = "auto_hunt_mode";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>()
            {
                {"level", levelIndex}
            });
#endif
        }
        
        public static void OnAnimalOfferAccepted()
        {
#if HAS_SDK
            const string eventName = "map_animal_offer_accepted";
#if UNITY_EDITOR || LOG_IN_BUILD
            CLog.Log($"=== === [Analytics] {eventName}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, new Dictionary<string, object>()
            { });
#endif
        }

    }
}