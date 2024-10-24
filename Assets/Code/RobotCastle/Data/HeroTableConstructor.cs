using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Data
{
    public class HeroTableConstructor : MonoBehaviour
    {
        [SerializeField] private bool _overrideExisting;
        [SerializeField] private TableReader _tableReader;
        [Space(10)]
        [SerializeField] private string _heroToGenerate;
        [SerializeField] private string _pathToHeroes;
        [SerializeField, Tooltip("Editor only. Use to copy")] private HeroInfo _heroTemplate;
        [Space(20)]
        [SerializeField] private string _enemyToGenerate;
        [SerializeField] private string _pathToEnemies;
        [SerializeField, Tooltip("Editor only. Use to copy")] private HeroInfo _enemyTemplate;
        [Space(20)] 
        [Header("Manual Generation Of Enemies")]
        [SerializeField] private string _enemyID;
        [SerializeField] private string _enemyName;
        [SerializeField] private List<string> _enemySkills;
        [SerializeField] private float _enemyHP;
        [SerializeField] private float _enemySP;
        [SerializeField] private float _enemyATK;
        [SerializeField] private List<float> _enemyATKSpeed;
        [SerializeField] private float _enemyMoveSpeed = 150;
        

        [ContextMenu("MakeHero")]
        public void MakeHero()
        {
            Generate(_heroToGenerate, _pathToHeroes, _heroTemplate);
        }

        [ContextMenu("MakeEnemy")]
        public void MakeEnemy()
        {
            Generate(_enemyToGenerate, _pathToEnemies, _enemyTemplate);
        }

        [ContextMenu("MakeEnemyFromScratch")]
        public void MakeEnemyFromScratch()
        {
#if UNITY_EDITOR
            var id = _enemyID;
            var pathToAssets = Application.streamingAssetsPath;
            pathToAssets = pathToAssets.Split("Assets")[0];
            var path = Path.Join(pathToAssets, _pathToEnemies,$"{id}.json");
            CLog.Log($"Path: {path}");
            var heroData = new HeroInfo();
            heroData.viewInfo = new HeroViewInfo();
            
            heroData.viewInfo.name = _enemyName;
            heroData.viewInfo.iconId = $"hero_icon_{id}";
            heroData.spellInfo = new HeroSpellInfo();
            if (_enemySkills.Count >= 1)
                heroData.spellInfo.mainSpellId = _enemySkills[0];
            if (_enemySkills.Count >= 2)
                heroData.spellInfo.secondSpellId = _enemySkills[1];
            if (_enemySkills.Count >= 3)
                heroData.spellInfo.thirdSpellId = _enemySkills[2];
            
            var stats = new HeroStats(); 
            heroData.stats = stats;
            
            stats.rangeId = id;
            const int statCount = 20;
            stats.moveSpeed = new List<float>(statCount);
            stats.attack = new List<float>(statCount);
            stats.health = new List<float>(statCount);
            stats.spellPower = new List<float>(statCount);

            for (var i = 0; i < statCount; i++)
            {
                stats.attack.Add(_enemyATK);
                stats.health.Add(_enemyHP);
                stats.spellPower.Add(_enemySP);
            }
            stats.moveSpeed.Add(_enemyMoveSpeed);
            stats.evasion = new List<float>() { 0 };
            stats.magicalCritChance = new List<float>() { 0 };
            stats.magicalCritDamage = new List<float>() { 0 };
            stats.physicalCritChance = new List<float>() { 0 };
            stats.physicalCritDamage = new List<float>() { 0 };
            stats.physHpDrain = new List<float>() { 0 };
            stats.magicHpDrain = new List<float>() { 0 };
            stats.magicalResist = new List<float>() { 0 };
            stats.physicalResist = new List<float>() { 0 };

            stats.attackSpeed = new List<float>(_enemyATKSpeed);
            if(_enemyATKSpeed.Count < 7)
                CLog.LogError("_enemyATKSpeed count < 7. There are 7 merge levels !");
            
            var obj = JsonConvert.SerializeObject(heroData, Formatting.Indented);
            File.WriteAllText(path, obj);
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        
        public void Generate(string id, string folderPath, HeroInfo template)
        {
            #if UNITY_EDITOR
            var pathToAssets = Application.streamingAssetsPath;
            pathToAssets = pathToAssets.Split("Assets")[0];
            var path = Path.Join(pathToAssets, folderPath,$"{id}.json");
            CLog.Log($"Path: {path}");
            if (File.Exists(path) && !_overrideExisting)
            {
                CLog.LogRed($"File for: {id} already exists");
                return;
            }

            var heroData = new HeroInfo(template);
            var viewName = char.ToUpper(id[0]).ToString() + id.Substring(1);
            heroData.viewInfo.name = viewName;
            heroData.viewInfo.iconId = $"hero_icon_{id}";
            var tableStats = _tableReader.ReadStatsForHero(id);
            if (tableStats != null)
                heroData.stats = tableStats;
            var obj = JsonConvert.SerializeObject(heroData, Formatting.Indented);
            File.WriteAllText(path, obj);
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }
    }
}