﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class EnemiesFactory : MonoBehaviour
    {
        public static EnemyPackPreset GetPackPreset(string presetPath)
        {
            var asset = Resources.Load<TextAsset>($"enemy_presets/{presetPath}");
            if (asset == null)
            {
                CLog.LogError($"{presetPath} DIDN'T find the preset!");
                return null;
            }
            var presetStr = asset.text;
            var preset = Newtonsoft.Json.JsonConvert.DeserializeObject<EnemyPackPreset>(presetStr);
            if (preset == null)
            {
                CLog.LogError($"[{nameof(EnemiesFactory)}] Cannot Deserialize preset at {presetPath}");
            }
            return preset;
        }
        
        public List<IHeroController> SpawnedEnemies => _spawnedEnemies;
        
        public IGridView GridView { get; set; }

        public async Task SpawnPresetEnemies(EnemyPackPreset packPreset, EliteItemsPreset items, CancellationToken token)
        {
            _spawnedEnemies = new List<IHeroController>(packPreset.enemies.Count);
            var factory = ServiceLocator.Get<IMergeItemsFactory>();
            var barsPanel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            var startInd = 0;
            for (var ind = startInd; ind < packPreset.enemies.Count; ind++)
            {
                var enemyPreset = packPreset.enemies[ind];
                var cellView = GridView.GetCell(enemyPreset.gridPos.x, enemyPreset.gridPos.y);
                var itemView = factory.SpawnItemOnCell(cellView, new ItemData(enemyPreset.enemy));
                var hero = itemView.Transform.GetComponent<IHeroController>();
                switch (enemyPreset.unitType)
                {
                    case EUnitType.Default or EUnitType.Elite:
                        barsPanel.AssignEnemyUI(hero.Components);
                        AnimateSpawn(hero);
                        break;
                    case EUnitType.Boss:
                        barsPanel.AssignBossUI(hero.Components);
                        hero.Components.animator.Play("Appear", 0, 0);
                        break;
                }
                hero.Components.weaponsContainer.SetEmpty();
                var spells = HeroesManager.GetModifiers(enemyPreset.modifiers);
                if (spells.Count == 0)
                {
                    spells = GetSpells(enemyPreset.enemy.id);
                }
                hero.InitHero(enemyPreset.enemy.id, enemyPreset.heroLevel, enemyPreset.enemy.level, spells);
                
                // CLog.LogGreen($"Random item?! {enemyPreset.giveRandomItem}. Item options Count: {items.itemsOptions.Count}");
                if (enemyPreset.unitType is EUnitType.Elite || enemyPreset.giveRandomItem)
                {
                    SetItems(hero, items);
                    hero.Components.weaponsContainer.CanDrop = enemyPreset.canDropItems;
                }
                hero.Components.heroUI.AssignStatsTracking(hero.Components);
                _spawnedEnemies.Add(hero);

                await Task.Yield();
                if (token.IsCancellationRequested) return;
            }
        }

        public IHeroController SpawnNew(SpawnArgs args, int heroLvl = 0, bool animate = true)
        {
            ICellView cellView = null;
            if (args.usePreferredCoordinate)
            {
                var cell = GridView.GetCell(args.preferredCoordinated.x, args.preferredCoordinated.y);
                if (cell.cell.isUnlocked == false || cell.cell.isOccupied)
                    return null;
                cellView = cell;
            }

            if (cellView == null)
            {
                foreach (var cell in GridView.Grid)
                {
                    if (cell.cell.isUnlocked && cell.cell.isOccupied == false)
                    {
                        cellView = cell;
                        break;
                    }
                }        
            }
            var barsPanel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            var factory = ServiceLocator.Get<IMergeItemsFactory>();
            var itemView = factory.SpawnItemOnCell(cellView, new ItemData(args.coreData));
            var hero = itemView.Transform.GetComponent<IHeroController>();
            barsPanel.AssignEnemyUI(hero.Components);
            List<ModifierProvider> spells = null;
            if (args.overrideSpells)
                spells = HeroesManager.GetModifiers(args.modifiersIds);
            else
            {
                spells = GetSpells(args.coreData.id);
            }
            hero.InitHero(args.coreData.id, heroLvl, args.coreData.level, spells);
            hero.Components.heroUI.AssignStatsTracking(hero.Components);
            if(animate)
                AnimateSpawn(hero);
            return hero;
        }
            
        
        [SerializeField] private string _presetsDirectory;
        private List<IHeroController> _spawnedEnemies;
        
        private List<ModifierProvider> GetSpells(string id)
        {
            var db = ServiceLocator.Get<HeroesDatabase>();
            var stats = db.GetHeroSpellInfo(id);
            var ids = new List<string>(3);
            if(!string.IsNullOrEmpty( stats.mainSpellId))
                ids.Add(stats.mainSpellId);
            if(!string.IsNullOrEmpty(stats.secondSpellId))
                ids.Add(stats.secondSpellId);
            if(!string.IsNullOrEmpty(stats.thirdSpellId))
                ids.Add(stats.thirdSpellId);
            return HeroesManager.GetModifiers(ids);
        }
        
        private void SetItems(IHeroController hero, EliteItemsPreset itemsPreset)
        {
            if (itemsPreset == null || itemsPreset.itemsOptions.Count == 0)
            {
                CLog.Log($"Items Preset is null or empty");
                hero.Components.weaponsContainer.SetEmpty();
                return;
            }

            var count = UnityEngine.Random.Range(itemsPreset.itemsCountMin, itemsPreset.itemsCountMax);
            var items = new List<CoreItemData>(3);
            var it = 0;
            const int it_max = 100;
            while (items.Count < count && it < it_max)
            {
                var random = itemsPreset.itemsOptions.Random();
                if (items.Contains(random))
                {
                    if (items.Count >= itemsPreset.itemsOptions.Count)
                        break;
                }
                else
                    items.Add(random);
                it++;
            }
            if (it >= it_max)
                CLog.LogError($"Too many iterations. Error trying to add random items");
            
            var weaponsData = ServiceLocator.Get<ModifiersDataBase>().GetWeaponsWithModifiers(items);
            hero.Components.weaponsContainer.SetItems(weaponsData);
            hero.Components.weaponsContainer.AddAllModifiersToHero(hero.Components);
        }
        
        private void AnimateSpawn(IHeroController hero)
        {
            var tr = hero.Components.transform;
            var pos = tr.position;
            var startPos = pos;
            pos.y += 10;
            tr.position = pos;
            tr.DOMove(startPos, .3f).SetEase(Ease.InQuad);
            var particles = ServiceLocator.Get<SimplePoolsManager>();
            var p = particles.GetOne("hero_spawn") as OneTimeParticles;
            p.Show(startPos);
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