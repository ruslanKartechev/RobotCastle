﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IHeroInitializer
    {
        void InitHero(IHeroController hero);
    }
    public class EnemiesFactory : MonoBehaviour
    {
        [SerializeField] private string _presetsDirectory;
        private List<IHeroController> _spawnedEnemies;
        
        public List<IHeroController> SpawnedEnemies => _spawnedEnemies;
        
        public IGridView GridView { get; set; }
        
        
        public async Task SpawnPreset(string presetPath, bool bossMode, CancellationToken token)
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

            if (bossMode)
                await SpawnBossPreset(preset, token);
            else
                await SpawnPreset(preset, token);
        }

        public async Task SpawnBossPreset(EnemyPackPreset packPreset, CancellationToken token)
        {
            _spawnedEnemies = new List<IHeroController>(packPreset.enemies.Count);
            var factory = ServiceLocator.Get<IMergeItemsFactory>();
            var barsPanel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            var bossPreset = packPreset.enemies[0];
            {
                var cellView = GridView.GetCell(bossPreset.gridPos.x, bossPreset.gridPos.y);
                var itemView = factory.SpawnItemOnCell(cellView, new ItemData(bossPreset.enemy));
                var hero = itemView.Transform.GetComponent<IHeroController>();
                barsPanel.AssignBossUI(hero.View);
                                
                var modifiers = HeroesManager.GetModifiers(bossPreset.modifiers);
                hero.InitHero(bossPreset.enemy.id, bossPreset.heroLevel, bossPreset.enemy.level, modifiers);
                hero.View.heroUI.AssignStatsTracking(hero.View);
                _spawnedEnemies.Add(hero);
                hero.View.animator.Play("Appear", 0, 0);
            }
            for (var ind = 1; ind < packPreset.enemies.Count; ind++)
            {
                var enemyPreset = packPreset.enemies[ind];
                var cellView = GridView.GetCell(enemyPreset.gridPos.x, enemyPreset.gridPos.y);
                var itemView = factory.SpawnItemOnCell(cellView, new ItemData(enemyPreset.enemy));
                var hero = itemView.Transform.GetComponent<IHeroController>();
                barsPanel.AssignEnemyUI(hero.View);
                var modifiers = HeroesManager.GetModifiers(enemyPreset.modifiers);
                hero.InitHero(enemyPreset.enemy.id, enemyPreset.heroLevel, enemyPreset.enemy.level, modifiers);
                hero.View.heroUI.AssignStatsTracking(hero.View);
                _spawnedEnemies.Add(hero);
                AnimateSpawn(hero);
                await Task.Yield();
                if (token.IsCancellationRequested) return;
            }
        }
        
        public async Task SpawnPreset(EnemyPackPreset packPreset, CancellationToken token)
        {
            _spawnedEnemies = new List<IHeroController>(packPreset.enemies.Count);
            var factory = ServiceLocator.Get<IMergeItemsFactory>();
            var barsPanel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            foreach (var enemyPreset in packPreset.enemies)
            {
                var cellView = GridView.GetCell(enemyPreset.gridPos.x, enemyPreset.gridPos.y);
                var itemView = factory.SpawnItemOnCell(cellView, new ItemData(enemyPreset.enemy));
                var hero = itemView.Transform.GetComponent<IHeroController>();
                barsPanel.AssignEnemyUI(hero.View);
                var modifiers = HeroesManager.GetModifiers(enemyPreset.modifiers);
                hero.InitHero(enemyPreset.enemy.id,enemyPreset.heroLevel, enemyPreset.enemy.level, modifiers);
                hero.View.heroUI.AssignStatsTracking(hero.View);
                _spawnedEnemies.Add(hero);
                AnimateSpawn(hero);
                await Task.Yield();
                if (token.IsCancellationRequested) return;
            }
        }

        public IHeroController SpawnNew(SpawnMergeItemArgs args, int heroLvl = 0)
        {
            ICellView cellView = null;
            if (args.usePreferredCoordinate)
            {
                var cell = GridView.GetCell(args.preferredCoordinated.x, args.preferredCoordinated.y);
                if (cell.cell.isUnlocked == false || cell.cell.isOccupied)
                    return null;
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
            var barsPanel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            var factory = ServiceLocator.Get<IMergeItemsFactory>();
            var itemView = factory.SpawnItemOnCell(cellView, new ItemData(args.coreData));
            var hero = itemView.Transform.GetComponent<IHeroController>();
            barsPanel.AssignEnemyUI(hero.View);
            hero.InitHero(args.coreData.id, heroLvl, args.coreData.level, new List<ModifierProvider>());
            hero.View.heroUI.AssignStatsTracking(hero.View);
            AnimateSpawn(hero);
            return hero;
        }
        
        private void AnimateSpawn(IHeroController hero)
        {
            var tr = hero.View.transform;
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