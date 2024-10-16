﻿using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class CheatItemsSpawner : MonoBehaviour
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
        [Space(10)] 
        [SerializeField] private int _heroIndex;
        [SerializeField] private List<string> _heroIds;
        // [SerializeField] private 

        public string HeroID { get; set; }

        public int HeroLvl { get; set; }

        public void NextHero()
        {
            _heroIndex++;
            CorrectIndexAndSetId();
        }

        public void PrevHero()
        {
            _heroIndex--;
            CorrectIndexAndSetId();
        }

        public void NextHeroLvl()
        {
            HeroLvl++;
            if (HeroLvl >= MergeConstants.HeroMaxMergeLvl)
                HeroLvl = MergeConstants.HeroMaxMergeLvl - 1;
        }

        public void PrevHeroLvl()
        {
            HeroLvl--;
            if(HeroLvl < 0)
                HeroLvl = 0;
            
        }

        [ContextMenu("Spawn Chosen Hero")]
        public void SpawnChosenHero()
        {
            var coreItem = new CoreItemData(HeroLvl, HeroID, MergeConstants.TypeHeroes);
            SpawnMergeItem(coreItem, null);
        }

        public void SpawnChosenHero(Vector2Int cell)
        {
            var coreItem = new CoreItemData(HeroLvl, HeroID, MergeConstants.TypeHeroes);
            var view = SpawnMergeItem(coreItem, null);
            var mergeManager = ServiceLocator.Get<MergeManager>();
            
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
            SpawnMergeItem(_oneHero, _addItemsToSpawnedHero ? _itemsForHero : null);
#endif
        }

        [ContextMenu("Spawn One Item")]
        public void SpawnOneItem()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                CLog.Log("[TestBattleGridSpawner] Not in play mode!");
                return;
            }
            var view = SpawnMergeItem(new CoreItemData(_oneItem));
            if (view == null)
                CLog.Log("[TestBattleGridSpawner] SpawnOneItem failed!");
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

        public void SpawnArmor1() => SpawnMergeItem(new CoreItemData(0, "armor", MergeConstants.TypeWeapons));
        public void SpawnArmor2() => SpawnMergeItem(new CoreItemData(1, "armor", MergeConstants.TypeWeapons));
        public void SpawnArmor3() => SpawnMergeItem(new CoreItemData(2, "armor", MergeConstants.TypeWeapons));
        public void SpawnArmor4() => SpawnMergeItem(new CoreItemData(3, "armor", MergeConstants.TypeWeapons));

        
        public void SpawnSword1() => SpawnMergeItem(new CoreItemData(0, "sword", MergeConstants.TypeWeapons));
        public void SpawnSword2() => SpawnMergeItem(new CoreItemData(1, "sword", MergeConstants.TypeWeapons));
        public void SpawnSword3() => SpawnMergeItem(new CoreItemData(2, "sword", MergeConstants.TypeWeapons));
        public void SpawnSword4() => SpawnMergeItem(new CoreItemData(3, "sword", MergeConstants.TypeWeapons));

        public void SpawnStaff1() => SpawnMergeItem(new CoreItemData(0, "staff", MergeConstants.TypeWeapons));
        public void SpawnStaff2() => SpawnMergeItem(new CoreItemData(1, "staff", MergeConstants.TypeWeapons));
        public void SpawnStaff3() => SpawnMergeItem(new CoreItemData(2, "staff", MergeConstants.TypeWeapons));
        public void SpawnStaff4() => SpawnMergeItem(new CoreItemData(3, "staff", MergeConstants.TypeWeapons));

        public void SpawnBow1() => SpawnMergeItem(new CoreItemData(0, "bow", MergeConstants.TypeWeapons));
        public void SpawnBow2() => SpawnMergeItem(new CoreItemData(1, "bow", MergeConstants.TypeWeapons));
        public void SpawnBow3() => SpawnMergeItem(new CoreItemData(2, "bow", MergeConstants.TypeWeapons));
        public void SpawnBow4() => SpawnMergeItem(new CoreItemData(3, "bow", MergeConstants.TypeWeapons));

        public void SpawnXPBook1() => SpawnMergeItem(new CoreItemData(0, MergeConstants.UpgradeBookId, MergeConstants.TypeWeapons));
        public void SpawnXPBook2() => SpawnMergeItem(new CoreItemData(1, MergeConstants.UpgradeBookId, MergeConstants.TypeWeapons));
        public void SpawnXPBook3() => SpawnMergeItem(new CoreItemData(2, MergeConstants.UpgradeBookId, MergeConstants.TypeWeapons));
        
        public void SpawnPreset(List<CoreItemData> list)
        {
            if (list.Count == 0)
            {
                CLog.Log("Preset list is empty");
                return;
            }
            var spawner = ServiceLocator.Get<IHeroesAndItemsFactory>();
            if (spawner == null)
            {
                CLog.LogError("NO IBattleGridSpawner found!");
                return;
            }
            foreach (var core in list)
                SpawnMergeItem(core);
        }

        public IItemView SpawnMergeItem(CoreItemData data, List<CoreItemData> items = null)
        {
            return SpawnHeroOrItem(data, false, default, items);
        }
        
        public static IItemView SpawnHeroOrItem(CoreItemData data, bool useSpecificCoord, Vector2Int coord, List<CoreItemData> items = null)
        {
   
            var manager = ServiceLocator.Get<MergeManager>();
            if (manager == null)
            {
                CLog.LogError("No MergeManager found!");
                return null;
            }
            var args = new SpawnMergeItemArgs(data);
            args.useAdditionalItems = (items is {Count: > 0});
            args.additionalItems = items;
            args.preferredCoordinated = coord;
            args.usePreferredCoordinate = useSpecificCoord;
            if (ServiceLocator.GetIfContains(out IPlayerMergeItemsFactory spawner))
                return spawner.SpawnHeroOrItem(args);
            
            CLog.LogRed($"IPlayerMergeItemPurchaser not found");
            return null;
        }
        
        private void CorrectIndexAndSetId()
        {
            if (_heroIds.Count > 0)
            {
                _heroIndex = Mathf.Clamp(_heroIndex, 0, _heroIds.Count - 1);
                HeroID = _heroIds[_heroIndex];
            }
            else
                _heroIndex = 0;
        }

        private void OnValidate()
        {
            if(_heroIds == null)
                _heroIds = new List<string>(5);
            CorrectIndexAndSetId();    
        }

        private void OnEnable()
        {
            CorrectIndexAndSetId();
        }
    }
}