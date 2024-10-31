using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.InvasionMode
{
    public class ProgressionManager
    {
        public static void CompleteTier(int chapterIndex, int tier, out bool completedFirstTime)
        {
            CLog.LogGreen($"Chapter: {chapterIndex+1}, tier: {tier+1} completed!");
            var db = ServiceLocator.Get<ProgressionDataBase>();
            var playerData = DataHelpers.GetPlayerData();
            var chapterSave = playerData.progression.chapters[chapterIndex];
            var tierSave = chapterSave.tierData[tier];
            completedFirstTime = !tierSave.completed;
            tierSave.completed = true;
            tierSave.completedCount++;

            var nextChapterInd = chapterIndex + 1;
            if (nextChapterInd >= db.chapters.Count)
                return;

            if (tier >= db.chapters[nextChapterInd].prevTierRequired)
            {
                var nextChapterSave = playerData.progression.chapters[nextChapterInd];
                nextChapterSave.unlocked = true;
                nextChapterSave.tierData[0].unlocked = true;   
            }

            var nextTier = tier + 1;
            if (nextTier < chapterSave.tierData.Count)
            {
                chapterSave.tierData[nextTier].unlocked = true;
            }
        }
    }
}