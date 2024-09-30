using System.IO;
using Newtonsoft.Json;
using RobotCastle.Battling;
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

        public void MakeHero()
        {
            Generate(_heroToGenerate, _pathToHeroes, _heroTemplate);
        }

        public void MakeEnemy()
        {
            Generate(_enemyToGenerate, _pathToEnemies, _enemyTemplate);
        }
        
        
        public void Generate(string id, string folderPath, HeroInfo template)
        {
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
            var obj = JsonConvert.SerializeObject(heroData);
            File.WriteAllText(path, obj);
        }
    }
}