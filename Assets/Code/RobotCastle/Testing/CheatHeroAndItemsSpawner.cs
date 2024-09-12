using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class CheatHeroAndItemsSpawner : MonoBehaviour
    {
        [Header("Hero")]
        [SerializeField] private CoreItemData _oneHero;
        [SerializeField] private bool _addItemsToSpawnedHero;
        [SerializeField] private List<CoreItemData> _itemsForHero;
        [Header("UnitItem")]
        [SerializeField] private CoreItemData _oneItem;
        [Header("List")]
        [SerializeField] private List<CoreItemData> _itemsPreset1;
        [SerializeField] private List<CoreItemData> _itemsPreset2;
        [SerializeField] private List<CoreItemData> _itemsPreset3;


        [ContextMenu("Spawn One Hero")]
        public void SpawnOneHero()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner] Not in play mode!");
                return;
            }
            
            SpawnItem(_oneHero, _itemsForHero);
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
            var did = battleGridSpawner.SpawnHero(_oneItem, out var view);
            if (!did)
                CLog.LogRed("[TestBattleGridSpawner] Did not spawn hero!");
#endif
        }
        
        [ContextMenu("Spawn Preset 1")]
        public void SpawnPreset1()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner]Not in play mode!");
                return;
            }
            SpawnPreset(_itemsPreset1);
#endif
        }
        
        [ContextMenu("Spawn Preset 2")]
        public void SpawnPreset2()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner]Not in play mode!");
                return;
            }
            SpawnPreset(_itemsPreset2);
#endif
        }

        [ContextMenu("Spawn Preset 3")]
        public void SpawnPreset3()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner]Not in play mode!");
                return;
            }
            SpawnPreset(_itemsPreset2);
#endif
        }

        public void SpawnArmor1() => SpawnItem(new CoreItemData(0, "armor", MergeConstants.TypeItems));
        public void SpawnArmor2() => SpawnItem(new CoreItemData(1, "armor", MergeConstants.TypeItems));
        public void SpawnArmor3() => SpawnItem(new CoreItemData(2, "armor", MergeConstants.TypeItems));
        public void SpawnArmor4() => SpawnItem(new CoreItemData(3, "armor", MergeConstants.TypeItems));

        
        public void SpawnSword1() => SpawnItem(new CoreItemData(0, "sword", MergeConstants.TypeItems));
        public void SpawnSword2() => SpawnItem(new CoreItemData(1, "sword", MergeConstants.TypeItems));
        public void SpawnSword3() => SpawnItem(new CoreItemData(2, "sword", MergeConstants.TypeItems));
        public void SpawnSword4() => SpawnItem(new CoreItemData(3, "sword", MergeConstants.TypeItems));

        public void SpawnStaff1() => SpawnItem(new CoreItemData(0, "staff", MergeConstants.TypeItems));
        public void SpawnStaff2() => SpawnItem(new CoreItemData(1, "staff", MergeConstants.TypeItems));
        public void SpawnStaff3() => SpawnItem(new CoreItemData(2, "staff", MergeConstants.TypeItems));
        public void SpawnStaff4() => SpawnItem(new CoreItemData(3, "staff", MergeConstants.TypeItems));

        public void SpawnBow1() => SpawnItem(new CoreItemData(0, "bow", MergeConstants.TypeItems));
        public void SpawnBow2() => SpawnItem(new CoreItemData(1, "bow", MergeConstants.TypeItems));
        public void SpawnBow3() => SpawnItem(new CoreItemData(2, "bow", MergeConstants.TypeItems));
        public void SpawnBow4() => SpawnItem(new CoreItemData(3, "bow", MergeConstants.TypeItems));

        public void SpawnXPBook1() => SpawnItem(new CoreItemData(0, "book_xp", MergeConstants.TypeItems));
        public void SpawnXPBook2() => SpawnItem(new CoreItemData(1, "book_xp", MergeConstants.TypeItems));
        public void SpawnXPBook3() => SpawnItem(new CoreItemData(2, "book_xp", MergeConstants.TypeItems));
        
        public void SpawnPreset(List<CoreItemData> list)
        {
            if (list.Count == 0)
            {
                CLog.Log("Preset list is empty");
                return;
            }
            var battleGridSpawner = ServiceLocator.Get<IBattleGridSpawner>();
            if (battleGridSpawner == null)
            {
                CLog.LogError("NO IBattleGridSpawner found!");
                return;
            }
            foreach (var itemData in list)
                battleGridSpawner.SpawnHero(itemData, out var view);
        }
        
        public void SpawnItem(CoreItemData data, List<CoreItemData> items = null)
        {
            var battleGridSpawner = ServiceLocator.Get<IBattleGridSpawner>();
            if (battleGridSpawner == null)
            {
                CLog.LogError("NO IBattleGridSpawner found!");
                return;
            }

            var did = false;
            if (items == null)
                did = battleGridSpawner.SpawnHero(data, out var view);
            else
                did = battleGridSpawner.SpawnHeroWithItems(data, items, out var view);
            if (!did)
                CLog.LogRed("[CheatSpawner] Did not spawn hero!");        
        }
        
        
    }
}