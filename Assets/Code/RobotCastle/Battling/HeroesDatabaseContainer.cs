using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Heroes Database Container", fileName = "Heroes Database Container", order = 0)]
    public class HeroesDatabaseContainer : ScriptableObject
    {
        [SerializeField] private List<string> _heroIdsForFiles;
        
        #if UNITY_EDITOR
        [SerializeField, Tooltip("Editor only. Use to copy")] private HeroInfo _dbgHero;
        #endif
        private HeroesDatabase _dataBase;

        public HeroesDatabase DataBase => _dataBase;

        public void Load()
        {
            var data = new HeroesDatabase();
            
            foreach (var id in _heroIdsForFiles)
            {
                var text = UnityEngine.Resources.Load<TextAsset>($"heroes/{id}");
                if (text == null)
                {
                    CLog.LogError($"Failed to find file {id}.json");
                    continue;
                }
                var info = JsonConvert.DeserializeObject<HeroInfo>(text.text);
                data.info.Add(id, info);
            }
            
            _dataBase = data;
// #if UNITY_EDITOR
//             try
//             {
//                 _dbgHero = data.info.First().Value;
//             }
//             catch (System.Exception ex) {
//                 CLog.LogError(ex.Message);
//             }
//             #endif
        }
        
        
#if UNITY_EDITOR
        
        public static string GetPathToFile(string fileName)
        {
            var path = Application.streamingAssetsPath;
            path = path.Replace("StreamingAssets", "Resources");
            path = Path.Join(path, "heroes",$"{fileName}.json");
            return path;
        }

        public void CreateForEveryId()
        {
            foreach (var id in _heroIdsForFiles)
            {
                var fileName = $"{GetPathToFile(id)}";
                var exists = File.Exists(fileName);
                if (exists)
                {
                    CLog.LogRed($"File {fileName} Already exists will SKIP");
                    continue;
                }
                CreateFile(id);
            }
        }

        public void CreateForEveryIdForced()
        {
            foreach (var id in _heroIdsForFiles)
            {
                var fileName = $"{GetPathToFile(id)}";
                var exists = File.Exists(fileName);
                if (exists)
                    CLog.LogWhite($"File {fileName} Already exists. WILL OVERRIDE");
                CreateFile(id);
            }
        }

        private void CreateFile(string id)
        {
            var heroInfo = new HeroInfo(_dbgHero);
            heroInfo.viewInfo.name = string.Concat(id[0].ToString().ToUpper(), id.Substring(1));
            heroInfo.viewInfo.iconId = $"hero_icon_{id}";
            var str = JsonConvert.SerializeObject(heroInfo, Formatting.Indented);
            File.WriteAllText(GetPathToFile(id),str);
        }

        [ContextMenu("Create New File")]
        public void CreateNewFile()
        {
           var id = "aramis";
           var stats = new HeroStats()
           {
                health = new List<float>() { 150, 180, 210, 285, 315, 345, 375, 450, 480, 510, 540, 570, 600, 630, 660, 705, 750, 795, 840, 885 },
                spellPower = new List<float>() { 70, 84, 98, 133, 147, 161, 175, 210, 224, 238, 252, 266, 280, 294, 308, 329, 350, 371, 392, 413 },
                attack = new List<float>() { 45, 54, 63, 86, 95, 104, 113, 135, 144, 153, 162, 171, 180, 189, 198, 212, 225, 239, 252, 266 },
                mana = new List<float>() {70, 80, 90, 100, 110, 120, 130},
                attackSpeed = new List<float>() { 67f, 74f, 80f, 87f, 94f, 101f, 107f },
                moveSpeed = new List<float>() { 150f },
                rangeId = "aramis"
           };
           var heroInfo = new HeroInfo();
           heroInfo.stats = stats;
           heroInfo.viewInfo = new HeroViewInfo() {name = "Aramis", iconId = "hero_icon_aramis"};
           heroInfo.spellInfo = new HeroSpellInfo() {mainSpellId = "Headshot"};
           var str = JsonConvert.SerializeObject(heroInfo, Formatting.Indented);
           File.WriteAllText(GetPathToFile(id),str);
        } 
   
#endif

    }
}