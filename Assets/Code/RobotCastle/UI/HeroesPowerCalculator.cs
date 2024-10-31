using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.Saving;
using SleepDev;
using UnityEngine;

namespace RobotCastle.UI
{
    public static class HeroesPowerCalculator
    {
        
        public static int CalculateTotalPlayerPower(List<string> heroes)
        {
            var db = ServiceLocator.Get<HeroesDatabase>();
            var saves = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
            var total = 0;
            var mergeLevels = 4;
            for (var mergeLvl = 0; mergeLvl < mergeLevels; mergeLvl++)
            {
                for (var i = 0; i < heroes.Count; i++)
                {
                    var id = heroes[i];
                    var lvl = saves.GetSave(id).level;
                    var stats = db.info[id].stats;
                    var atk = HeroStatsManager.GetStatByLevel(stats.attack, lvl, mergeLvl);
                    var atks = HeroStatsManager.GetStatByMergeLevel(stats.attackSpeed, mergeLvl);
                    var health = HeroStatsManager.GetStatByLevel(stats.health, lvl, mergeLvl) / 10;
                    var def = (stats.physicalResist[0] + stats.magicalResist[0]);
                    if(def > 0)
                        health *= (1 / HeroesManager.GetDef(def));
                    total += (int)(atk * atks + health);
                }
            }
            total /= mergeLevels;
            return total;
        }
        
        
        public static int CalculateTotalPowerForEnemies(LevelData preset, int difficulty)
        {
            var db = ServiceLocator.Get<HeroesDatabase>();
            var overallTotal = 0f;
            var roundsCount = preset.levels.Count;
            var difficultyDb = ServiceLocator.Get<DifficultyTiersDatabase>();
            var tierConfig = difficultyDb.tiersConfig[difficulty]; 
            
            foreach (var roundData in preset.levels)
            {
                var roundTotal = 0f;
                var pack = EnemiesFactory.GetPackPreset(roundData.enemyPreset);
                if (tierConfig.enemyForcesPercent > 0)
                {
                    var enemy = pack.enemies[0];
                    var count = Mathf.RoundToInt(tierConfig.enemyForcesPercent * pack.enemies.Count);
                    if (count == 0)
                        count = 1;
                    for(var i = 0; i < count; i++)
                        pack.enemies.Add(new EnemyPreset(enemy));
                }
                foreach (var enPreset in pack.enemies)
                {
                    var heroTotal = 0;
                    var id = enPreset.enemy.id;
                    if (string.IsNullOrEmpty(id))
                    {
                        CLog.LogError("Id is null or empty");
                        continue;
                    }
                    var lvl = enPreset.heroLevel;
                    if(db.info.ContainsKey(id) == false)
                    {
                        CLog.LogRed($"Heroes database does not contain: {id}");
                        continue;
                    }
                    var stats = db.info[id].stats;
                    var mergeLvl = enPreset.enemy.level + tierConfig.enemyTier;
                    if (mergeLvl >= 7)
                        mergeLvl = 7;
                    var atk = HeroStatsManager.GetStatByLevel(stats.attack, lvl, mergeLvl);
                    var atks = HeroStatsManager.GetStatByMergeLevel(stats.attackSpeed, mergeLvl);
                    var health = HeroStatsManager.GetStatByLevel(stats.health, lvl, mergeLvl) / 10;
                    // var orHealth = health;
                    var def = (stats.physicalResist[0] + stats.magicalResist[0]);
                    if(def > 0)
                        health *= 1 / HeroesManager.GetDef(def);
                    heroTotal += (int)(atk * atks + health);
                    // CLog.Log($"{id}. Atk: {atk}, atks: {atks}, health: {orHealth}, def {def}, healthMod {health},   total power: {heroTotal}");
                    roundTotal += heroTotal;
                }
                overallTotal += roundTotal;
            }
            overallTotal /= roundsCount;
            return (int)overallTotal;
        }

        
        
    }
}