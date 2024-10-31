using System.Collections.Generic;

namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class SaveInvasionProgression
    {
        public int chapterIndex; 
        public int tierLevel;
        public List<ChapterData> chapters;
        public List<ChapterData> chaptersCorruption;
        
        public SaveInvasionProgression(){}

        public SaveInvasionProgression(SaveInvasionProgression other)
        {
            chapterIndex = other.chapterIndex;
            tierLevel = other.tierLevel;
            var count = other.chapters.Count;
            chapters = new List<ChapterData>(count);
            for (var i = 0; i < count; i++)
                chapters.Add(new ChapterData(other.chapters[i]));
            
            count = other.chaptersCorruption.Count;
            chaptersCorruption = new List<ChapterData>(count);
            for (var i = 0; i < count; i++)
                chaptersCorruption.Add(new ChapterData(other.chaptersCorruption[i]));

        }

        public void SetFirstTierUnlockedIfChapterUnlocked()
        {
            foreach (var chapter in chapters)
            {
                if (chapter.unlocked)
                    chapter.tierData[0].unlocked = true;
            }
        }

        public TierData GetTierData(int chapterInd, int tierInd)
        {
            if (chapterInd > chapters.Count)
                return null;
            if (tierInd >= chapters[chapterInd].tierData.Count)
                return null;
            return chapters[chapterInd].tierData[tierInd];
        }

        [System.Serializable]
        public class ChapterData
        {
            public bool unlocked;
            public List<TierData> tierData;
            
            public ChapterData(){}

            public ChapterData(ChapterData other)
            {
                unlocked = other.unlocked;
                var count = other.tierData.Count;
                tierData = new (count);
                for (var i = 0; i < count; i++)
                    tierData.Add(new TierData(other.tierData[i]));
            }
        }

        [System.Serializable]
        public class TierData
        {
            public bool unlocked;
            public bool completed;
            public int completedCount;
            public int attemptedCount;
            
            public TierData(){}

            public TierData(TierData other)
            {
                unlocked = other.unlocked;
                completed = other.completed;
                completedCount = other.completedCount;
                attemptedCount = other.attemptedCount;
            }
        }
    }
}