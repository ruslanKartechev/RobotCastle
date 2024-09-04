using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class TestBattleGridSpawner : MonoBehaviour
    {
        [Header("Hero")]
        [SerializeField] private CoreItemData _oneHero;
        [SerializeField] private WeightedList<CoreItemData> _randomOptions;
        [Header("Items")]
        [SerializeField] private CoreItemData _oneItem;
        [SerializeField] private WeightedList<CoreItemData> _randomItems;
        [Header("List")]
        [SerializeField] private List<CoreItemData> _itemsList;
        

        [ContextMenu("Spawn Random Hero")]
        public void SpawnRandomHero()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("Not in play mode!");
                return;
            }
            var battleGridSpawner = ServiceLocator.Get<IBattleGridSpawner>();
            var did = battleGridSpawner.SpawnRandomHero(_randomOptions);
            if (!did)
                CLog.LogRed("[TestBattleGridSpawner] Did not spawn random hero!");
            #endif
        }

        [ContextMenu("Spawn One Hero")]
        public void SpawnOneHero()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner] Not in play mode!");
                return;
            }
            var battleGridSpawner = ServiceLocator.Get<IBattleGridSpawner>();
            var did = battleGridSpawner.SpawnHero(_oneHero);
            if (!did)
                CLog.LogRed("[TestBattleGridSpawner] Did not spawn hero!");
#endif
        }
        
        [ContextMenu("Spawn Random Item")]
        public void SpawnRandomItem()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner] Not in play mode!");
                return;
            }
            var battleGridSpawner = ServiceLocator.Get<IBattleGridSpawner>();
            var did = battleGridSpawner.SpawnRandomHero(_randomItems);
            if (!did)
                CLog.LogRed("[TestBattleGridSpawner] Did not spawn random hero!");
#endif
        }

        [ContextMenu("Spawn One Item")]
        public void SpawnOneItem()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner]Not in play mode!");
                return;
            }
            var battleGridSpawner = ServiceLocator.Get<IBattleGridSpawner>();
            var did = battleGridSpawner.SpawnHero(_oneItem);
            if (!did)
                CLog.LogRed("[TestBattleGridSpawner] Did not spawn hero!");
#endif
        }
        
        [ContextMenu("Spawn Items List")]
        public void SpawnItemsList()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner]Not in play mode!");
                return;
            }
            var battleGridSpawner = ServiceLocator.Get<IBattleGridSpawner>();
            foreach (var itemData in _itemsList)
            {
                battleGridSpawner.SpawnHero(itemData);
            }
#endif
        }
    }
}