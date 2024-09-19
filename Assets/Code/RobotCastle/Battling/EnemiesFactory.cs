using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class EnemiesFactory : MonoBehaviour
    {
        [SerializeField] private string _presetsDirectory;
        private List<HeroController> _spawnedEnemies;
        
        public List<HeroController> SpawnedEnemies => _spawnedEnemies;
        
        public IGridView GridView { get; set; }

        public void SpawnPreset(string presetPath)
        {
            var asset = Resources.Load<TextAsset>($"enemy_presets/{presetPath}");
            if (asset == null)
            {
                CLog.LogError($"{presetPath} DIDN'T find the preset!");
                _spawnedEnemies = null;
                return;
            }
            var presetStr = asset.text;
            var preset = Newtonsoft.Json.JsonConvert.DeserializeObject<EnemyPackPreset>(presetStr);
            if (preset == null)
            {
                CLog.LogError($"[{nameof(EnemiesFactory)}] Cannot Deserialize preset at {presetPath}");
            }
            SpawnPreset(preset);
        }

        public void SpawnPreset(EnemyPackPreset packPreset)
        {
            _spawnedEnemies = new List<HeroController>(packPreset.enemies.Count);
            var factory = ServiceLocator.Get<IHeroesAndUnitsFactory>();
            foreach (var enemyPreset in packPreset.enemies)
            {
                var cellView = GridView.GetCell(enemyPreset.gridPos.x, enemyPreset.gridPos.y);
                var args = new SpawnMergeItemArgs(enemyPreset.enemy);
                if (enemyPreset.items is { Count : > 0 })
                {
                    args.useAdditionalItems = true;
                    args.additionalItems = enemyPreset.items;
                }
                factory.SpawnHeroOrItem(args, cellView, out var itemView);
                var enemy = itemView.Transform.GetComponent<HeroController>();
                enemy.transform.position = cellView.WorldPosition;
                enemy.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                enemy.InitComponents(enemyPreset.enemy.id, enemyPreset.heroLevel, enemyPreset.enemy.level);
                enemy.GetComponent<IItemView>().UpdateViewToData(new ItemData(enemyPreset.enemy));
                _spawnedEnemies.Add(enemy);
            }
        }

        public void SpawnNew(SpawnMergeItemArgs args)
        {
            ICellView cellView = null;
            if (args.usePreferredCoordinate)
            {
                var cell = GridView.GetCell(args.preferredCoordinated.x, args.preferredCoordinated.y);
                if (cell.cell.isUnlocked == false || cell.cell.isOccupied)
                    return;
                cellView = cell;
            }

            foreach (var cell in GridView.Grid)
            {
                if (cell.cell.isUnlocked && cell.cell.isOccupied == false)
                {
                    cellView = cell;
                    break;
                }
            }
            var factory = ServiceLocator.Get<IHeroesAndUnitsFactory>();
            factory.SpawnHeroOrItem(args, cellView, out var spawnedItem);

        }

#if UNITY_EDITOR
        [ContextMenu("E_CreateTestPreset")]
        public void E_CreateTestPreset()
        {
            var enemy1 = new EnemyPreset()
            {
                enemy = new CoreItemData(0, "orc_1", "unit"),
                gridPos = new Vector2Int(3, 1),
                items = new List<CoreItemData>()
            };
            var enemy2 = new EnemyPreset()
            {
                enemy = new CoreItemData(0, "orc_2", "unit"),
                gridPos = new Vector2Int(4, 1),
                items = new List<CoreItemData>()
            };
            var preset = new EnemyPackPreset()
            {
                enemies = new List<EnemyPreset>(){enemy1, enemy2}
            };
            var str = JsonConvert.SerializeObject(preset, Formatting.Indented);
            var path = Application.streamingAssetsPath;
            var endPath = $"/{_presetsDirectory}/ep_test_1.json";
            path = path.Replace("StreamingAssets", "Resources");
            path += endPath;
            File.WriteAllText(path:path, contents:str);
        }
        
        #endif
    }
}