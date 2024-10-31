using System;
using System.Collections.Generic;
using System.IO;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.InvasionMode
{
    [CreateAssetMenu(menuName = "SO/Invasion Progression", fileName = "Invasion Progression", order = -300)]
    public class ProgressionDataBaseContainer : ScriptableObject
    {
        public ProgressionDataBase database;

        
        
        
        #if UNITY_EDITOR

        [Space(20)] 
        public string rewards_config_file_path;

        [ContextMenu("1 Read Chapter Rewards")]
        public void ReadChapterRewards()
        {
            var perChapter = Read(rewards_config_file_path);
            for (var i = 0; i < perChapter.Count; i++)
            {
                var chapter = database.chapters[i];
                var chapterRewards = perChapter[i];
                for (var tierInd = 0; tierInd < chapter.tiers.Count; tierInd++)
                {
                    var tier = chapter.tiers[tierInd];
                    var tierRewards = chapterRewards[tierInd];
                    tier.additionalRewards = tierRewards;
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// List of chapters( tiers (rewards) )
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<List<List<CoreItemData>>> Read(string path)
        {
            if (!File.Exists(path))
            {
                CLog.LogRed($"File does not exist");
                return null;
            }
            List<List<List<CoreItemData>>> itemsPerChapter = new List<List<List<CoreItemData>>>(10);
            var reader = new StreamReader(path);
            var delimiter = '\t';
            var columns = reader.ReadLine().Split(delimiter);
            for (var rowInd = 1; reader.Peek() > 0; rowInd++)
            {
                var wholeLine = reader.ReadLine();
                // CLog.Log($"{rowInd} Line: {line}");
                if (wholeLine == null)
                    continue;
                columns = wholeLine.Split(delimiter);
                var didParse =false;
                var column = 0;
                int chapterNum = 0;
                for (; column < columns.Length; column++)
                {
                     var header = columns[column];
                     if (string.IsNullOrEmpty(header))
                         continue;
                     didParse = Int32.TryParse(header, out chapterNum);
                     break;
                }
                if (!didParse)
                    continue;
                CLog.LogGreen($"============= Chapter number: {chapterNum}");
                var chapterInd = chapterNum - 1;
                List<List<CoreItemData>> perTier = new List<List<CoreItemData>>(5);
                for (var tier = 0; tier < 5; tier++)
                {                
                    var rewards = new List<CoreItemData>(3);
                    var columnInd = column + 2 + tier;
                    var str = columns[columnInd];
                    if (string.IsNullOrEmpty(str))
                        continue;
                    // CLog.Log($"Tier {tier + 1}: {str}");
                    var lines = str.Split(',');
                    foreach (var temp in lines)
                    {
                        var line = temp;
                        while (line.Length > 0 && line[0] == ' ')
                        {
                            line = line.Remove(0, 1);
                        }

                        if (string.IsNullOrEmpty(line) || line.Length < 2)
                            continue;
                        
                        // CLog.Log($"ItemLine: {line}");
                        var entries = line.Split(' ');
                        if (entries.Length < 2)
                            continue;
                        var id = entries[0];
                        id = id.Replace(" ", "");
                        id = id.Replace(":", "");
                        var amountText = entries[1].Replace(" ", "");
                        amountText = amountText.Replace(":", "");
                        var amount = 0;
                        if (!Int32.TryParse(amountText, out amount))
                        {
                            CLog.LogRed($"Cannot parse amount: {amountText}");
                            continue;
                        }
                        rewards.Add(new CoreItemData(amount, id, "item"));
                    }
                    perTier.Add(rewards);
                }
                itemsPerChapter.Add(perTier);
                var totalCount = 0;
                foreach (var tt in perTier)
                    totalCount += tt.Count;
                CLog.LogGreen($"Chapter: {chapterInd+1}. Items count: {totalCount}");
            }
            reader.Close();
            return itemsPerChapter;
        }


        [ContextMenu("E_CalculateTotalPower")]
        public void E_CalculateTotalPower()
        {
            foreach (var chapter in database.chapters)
            {
                for (var i = 0; i < chapter.tiers.Count; i++)
                {
                    var tier = chapter.tiers[i];
                    var total = HeroesPowerCalculator.CalculateTotalPowerForEnemies(chapter.levelData, i);
                    tier.totalPower = total;
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        
        [ContextMenu("E_FitLevelTypes")]
        public void E_FitLevelTypes()
        {
            for (var chapterInd = 0; chapterInd < database.chapters.Count; chapterInd++)
            {
                if (chapterInd == 0)
                    continue;
                var chapter = database.chapters[chapterInd];
                var levels = chapter.levelData.levels;
                var count = levels.Count;
                for (var i = 0; i < count; i++)
                {
                    var lvl = i + 1;
                    if (lvl is 4 or 9 or 14 or 19)
                    {
                        levels[i].roundType = RoundType.Smelting;
                    }
                    else if (lvl is 5 or 10 or 15 or 20)
                    {
                        if (lvl == count)
                        {
                            levels[i].roundType = RoundType.Boss;
                            break;
                        }
                        else
                        {
                            levels[i].roundType = RoundType.EliteEnemy;
                        }
                    }
                    else
                    {
                        levels[i].roundType = RoundType.Default;
                    }
                }

            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("E_FitPresetIds")]
        public void E_FitPresetIds()
        {
            for (var chapterInd = 0; chapterInd < database.chapters.Count; chapterInd++)
            {
                if (chapterInd == 0)
                    continue;
                var chapter = database.chapters[chapterInd];
                var levels = chapter.levelData.levels;
                var idm = chapter.id;
                var count = levels.Count;
                for (var i = 0; i < count; i++)
                {
                    levels[i].enemyPreset = $"{idm}_{i + 1}";
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        #endif
        
        // IF 10,       IF 15,      IF 20
        // 1 - normal |
        // 2 - normal |
        // 3 - norm.+rew |
        // 4 - smelting |
        // 5 - elite |
        // 6 - normal |
        // 7 - normal |
        // 8 - normal |
        // 9 - normal |
        // 10 - normal |
        // 11 - normal |
        // 12 - normal |
        // 13 - normal |
        // 14 - normal |
        // 15 - normal |
        // 16 - normal |
        // 17 - normal |
        // 18 - normal |
        // 19 - normal |
        // 20 - normal |
        
        
    }
}