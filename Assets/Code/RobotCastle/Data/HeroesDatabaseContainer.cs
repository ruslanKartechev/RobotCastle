using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RobotCastle.Battling;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Data
{
    [CreateAssetMenu(menuName = "SO/Heroes Database Container", fileName = "Heroes Database Container", order = 0)]
    public class HeroesDatabaseContainer : ScriptableObject
    {
        public HeroesDatabase DataBase => _dataBase;
        
        public List<int> xpForLevels = new(20);
        
        [Space(10)]
        [SerializeField] private string pathToHeroes;
        [SerializeField] private string pathToEnemies;
        [Space(8)]
        [SerializeField] private List<string> _heroIdsForFiles;
        [SerializeField] private List<string> _enemiesIdsForFiles;
        private HeroesDatabase _dataBase;


        public void Load()
        {
            var data = new HeroesDatabase();
            foreach (var id in _heroIdsForFiles)
            {
                var text = UnityEngine.Resources.Load<TextAsset>($"{pathToHeroes}/{id}");
                if (text == null)
                {
                    CLog.LogError($"Failed to find file {id}.json");
                    continue;
                }
                var info = JsonConvert.DeserializeObject<HeroInfo>(text.text);
                data.info.Add(id, info);
            }
            foreach (var id in _enemiesIdsForFiles)
            {
                var text = UnityEngine.Resources.Load<TextAsset>($"{pathToEnemies}/{id}");
                if (text == null)
                {
                    CLog.LogError($"Failed to find file {id}.json");
                    continue;
                }
                var info = JsonConvert.DeserializeObject<HeroInfo>(text.text);
                data.info.Add(id, info);
            }
            data.xpForLevels = new List<int>(xpForLevels);
            _dataBase = data;
        }
        
        
#if UNITY_EDITOR

        public static string GetPathToFileHero(string fileName)
        {
            var path = Application.streamingAssetsPath;
            path = path.Replace("StreamingAssets", "Resources");
            path = Path.Join(path, "heroes",$"{fileName}.json");
            return path;
        }
        
        [Space(15)] 
        [SerializeField] public string e_id = "aramis";
        [SerializeField] public string e_folder = "heroes";
        [SerializeField] public float e_multiplier;
        [SerializeField] public EStatType e_statType;

        [ContextMenu("MultiplyStatForChosenId")]
        public void MultiplyStatForChosenId()
        {
            var path = GetPathFor(e_id, e_folder);
            if (File.Exists(path))
            {
                CLog.Log($"{path} File exists");
                var text = UnityEngine.Resources.Load<TextAsset>($"{e_folder}/{e_id}");
                if (text == null)
                {
                    CLog.LogError($"Failed to find file {e_id}.json");
                    return;
                }
                var info = JsonConvert.DeserializeObject<HeroInfo>(text.text);
                if (info == null)
                {
                    CLog.LogRed("Failed to deserialized");
                    return;
                }

                var mult = e_multiplier;
                switch (e_statType)
                {
                    case EStatType.Attack:
                        for (var i = 0; i < info.stats.attack.Count; i++)
                            info.stats.attack[i] *= mult;
                        break;
                    case EStatType.Health:
                        for (var i = 0; i < info.stats.health.Count; i++)
                            info.stats.health[i] *= mult;
                        break;
                    case EStatType.AttackSpeed:
                        for (var i = 0; i < info.stats.attackSpeed.Count; i++)
                            info.stats.attackSpeed[i] *= mult;
                        break;
                    default:
                        CLog.Log($"{e_statType.ToString()} is not supported");
                        return;
                }
                var str = JsonConvert.SerializeObject(info, Formatting.Indented);
                File.WriteAllText(GetPathFor(e_id, e_folder),str);
            }
            else
            {
                CLog.Log($"{path} File DOESN'T exists");
                return;
            }
        }

        private void CreateFile(string id, string folderPath, HeroInfo preset)
        {
            var heroInfo = new HeroInfo(preset);
            var vName = new string(id);
            heroInfo.viewInfo.name = string.Concat(vName[0].ToString().ToUpper(), vName.Substring(1));
            heroInfo.viewInfo.iconId = $"hero_icon_{id}";
            var str = JsonConvert.SerializeObject(heroInfo, Formatting.Indented);
            File.WriteAllText(GetPathFor(id, folderPath),str);
        }

        private static string GetPathFor(string id, string folder)
        {
            var path = Application.streamingAssetsPath;
            path = path.Replace("StreamingAssets", "Resources");
            path += $"/{folder}/{id}.json";
            return path;
        }


        [ContextMenu("Create New File")]
        public void CreateNewPlayerConfigFile()
        {
           var id = "evan";
           var stats = new HeroStats()
           {
                health = new List<float>() { 250, 300, 350, 285, 315, 345, 375, 450, 480, 510, 540, 570, 600, 630, 660, 705, 750, 795, 840, 885 },
                attack = new List<float>() { 25, 30, 35, 48, 53, 58, 63, 75, 80, 68, 72, 76, 80, 84, 88, 94, 100, 106, 112, 118 },
                spellPower = new List<float>() { 20, 24, 28, 38, 42, 46, 50, 60, 64, 68, 72, 76, 80, 84, 88, 94, 100, 106, 112, 118 },
                attackSpeed = new List<float>() { 1.2f, 1.32f, 1.44f, 1.56f, 1.68f, 180f, 192f },
                moveSpeed = new List<float>() { 150f },
                rangeId = "evan"
           };
           var heroInfo = new HeroInfo();
           heroInfo.stats = stats;
           heroInfo.viewInfo = new HeroViewInfo() {name = "Evan", iconId = "hero_evan"};
           heroInfo.spellInfo = new HeroSpellInfo() {mainSpellId = "crescent_slash"};
           var str = JsonConvert.SerializeObject(heroInfo, Formatting.Indented);
           File.WriteAllText(GetPathToFileHero(id),str);
        } 
   
#endif

    }
}