using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Heroes Database Container", fileName = "Heroes Database Container", order = 0)]
    public class HeroesDatabaseContainer : ScriptableObject
    {
        [SerializeField] private string pathToHeroes;
        [SerializeField] private string pathToEnemies;
        [Space(8)]
        [SerializeField] private List<string> _heroIdsForFiles;
        [SerializeField] private List<string> _enemiesIdsForFiles;

#if UNITY_EDITOR
        [Space(5)] [SerializeField, Tooltip("Editor only. Use to copy")] private HeroInfo _dbgHero;
        [Space(5)] [SerializeField, Tooltip("Editor only. Use to copy")] private HeroInfo _dbgEnemy;
#endif
        private HeroesDatabase _dataBase;

        public HeroesDatabase DataBase => _dataBase;

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

        public void CreateForEveryEnemyId()
        {
            foreach (var id in _enemiesIdsForFiles)
            {
                var fileName = GetPathFor(id, pathToEnemies);
                var exists = File.Exists(fileName);
                if (exists)
                {
                    CLog.LogRed($"File {fileName} Already exists will SKIP");
                    continue;
                }
                CreateFile(id, pathToEnemies, _dbgEnemy);
            }
        }

        public void CreateForEveryEnemyIdForced()
        {
            foreach (var id in _enemiesIdsForFiles)
            {
                var fileName = GetPathFor(id, pathToEnemies);
                var exists = File.Exists(fileName);
                if (exists)
                    CLog.LogWhite($"File {fileName} Already exists. WILL OVERRIDE");
                CreateFile(id, pathToEnemies, _dbgEnemy);
            }
        }
        
        public void CreateForEveryPlayerId()
        {
            foreach (var id in _heroIdsForFiles)
            {
                var fileName = GetPathFor(id, pathToHeroes);
                var exists = File.Exists(fileName);
                if (exists)
                {
                    CLog.LogRed($"File {fileName} Already exists will SKIP");
                    continue;
                }
                CreateFile(id, pathToHeroes, _dbgHero);
            }
        }

        public void CreateForEveryPlayerIdForced()
        {
            foreach (var id in _heroIdsForFiles)
            {
                var fileName = GetPathFor(id, pathToHeroes);
                var exists = File.Exists(fileName);
                if (exists)
                    CLog.LogWhite($"File {fileName} Already exists. WILL OVERRIDE");
                CreateFile(id, pathToHeroes, _dbgHero);
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

        [ContextMenu("FillDbgHero")]
        public void FillDbgHero()
        {
            _dbgHero.stats.physicalResist = new(20);
            _dbgHero.stats.magicalResist = new(20);

            var val = .010f;
            for (var i = 0; i < 20; i++)
            {
                _dbgHero.stats.physicalResist.Add(val);
                val += .010f;
            }
            val = .01f;
            for (var i = 0; i < 20; i++)
            {
                _dbgHero.stats.magicalResist.Add(val);
                val += .010f;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Create New File")]
        public void CreateNewPlayerConfigFile()
        {
           var id = "aramis";
           var stats = new HeroStats()
           {
                health = new List<float>() { 150, 180, 210, 285, 315, 345, 375, 450, 480, 510, 540, 570, 600, 630, 660, 705, 750, 795, 840, 885 },
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
           File.WriteAllText(GetPathToFileHero(id),str);
        } 
   
#endif

    }
}