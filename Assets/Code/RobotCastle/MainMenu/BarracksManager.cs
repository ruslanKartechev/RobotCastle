using System;
using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.Saving;
using SleepDev;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class BarracksManager : MonoBehaviour, IMergeProcessor, IGridSectionsController, ISwapAllowedCheck
    {
        [SerializeField] private bool _logActiveParty;
        [SerializeField] private GridView _gridView;
        [SerializeField] private MergeInput _mergeInput;
        [SerializeField] private BarrackUnitInfoPanel _unitInfoPanel;
        [SerializeField] private BarracksHeroViewInput _barracksInput;
        private bool _init;
        private MergeController _mergeController;
        private List<IItemView> _nonActive = new (20);
        private List<IItemView> _active = new (6);
        
        public void Init()
        {
            if (_init) return;
            _init = true;
            _gridView.BuildGridFromView();
            _mergeController = new MergeController(this,this, _gridView, this, null);
            _mergeController.useHighlight = false;
            _mergeInput.Init(_mergeController);
            _unitInfoPanel.Init(new Vector2Int(_gridView.Grid.GetLength(0), _gridView.Grid.GetLength(1)));
        }

        public void Show(bool allowInput = true)
        {
            _unitInfoPanel.gameObject.SetActive(true);
            _gridView.gameObject.SetActive(true);
            var party = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().party;
            var heroes = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
            var pool = ServiceLocator.Get<BarracksHeroesPool>();
            _active.Clear();
            _nonActive.Clear();            
            var rowCount = HeroesConstants.PartySize;
            for (var x = 0; x < rowCount; x++)
            {
                var id = party.heroesIds[x];
                if (string.IsNullOrEmpty(id))
                    continue;
                var hero = pool.GetHero(id);
                hero.Transform.gameObject.SetActive(true);
                var cell = _gridView.Grid[x, 0];
                MergeFunctions.PutItemToCell(hero, cell);
                _active.Add(hero);
            }
            var xInd = 0;
            var yInd = 1;
            for (var i = 0; i < heroes.heroSaves.Count; i++)
            {
                var heroData = heroes.heroSaves[i];
                var id = heroData.id;
                if(party.heroesIds.Contains(id))
                    continue;
                var hero = pool.GetHero(id);
                hero.Transform.gameObject.SetActive(true);
                var cell = _gridView.Grid[xInd, yInd];
                MergeFunctions.PutItemToCell(hero, cell);
                _nonActive.Add(hero);
                if (heroData.isUnlocked == false)
                {
                    
                }
                xInd++;
                if (xInd >= rowCount)
                {
                    xInd = 0;
                    yInd++;
                }
            }
            _unitInfoPanel.ShowForAllItemsOnGrid(_gridView);
            if (allowInput)
            {
                AllowMergeInput(true);
                _barracksInput.SetActive(true);
            }
        }

        public void Hide()
        {
            _barracksInput.SetActive(false);
            _unitInfoPanel.gameObject.SetActive(false);
            AllowMergeInput(false);
            _gridView.gameObject.SetActive(false);
            foreach (var view in _nonActive)
                view.Transform.gameObject.SetActive(false);
        }

        public void AllowMergeInput(bool allowed)
        {
            _mergeInput.SetActive(allowed);
            if (allowed)
            {
                _mergeController.OnPutItem -= OnMergeInputEnd;
                _mergeController.OnPutItem += OnMergeInputEnd;
            }
            else
            {
                _mergeController.OnPutItem -= OnMergeInputEnd;
            }
        }

        public bool IsCellAllowed(Vector2Int coord, ItemData item, bool promptUser = true)
        {
            if (coord.y == 0)
                return true;
            return false;
        }
        
        public void OnItemPut(ItemData item)
        {
            // CLog.Log($"On item put: {item.core.id}");
            var ui = _unitInfoPanel.GetUIForCell(item.pivotX, item.pivotY);
            ui.ShowInfo(item.core.id);
        }

        private void RewritePlayerParty()
        {
            var party = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().party;
            for (var i = 0; i < HeroesConstants.PartySize; i++)
            {
                var cell = _gridView.Grid[i, 0];
                if(cell.itemView == null)
                    party.heroesIds[i] = "";
                else
                    party.heroesIds[i] = cell.itemView.itemData.core.id;
            }
        }
        
        public bool IsSwapAllowed(ICellView cell1, ICellView cell2)
        {
            if (cell1.cell.y != 0 && cell2.cell.y != 0)
                return false;
            
            var db = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
            if (db.GetSave(cell1.itemView.itemData.core.id).isUnlocked == false)
                return false;
            if (db.GetSave(cell2.itemView.itemData.core.id).isUnlocked == false)
                return false;

            return true;
        }
        
        public void TryMerge(IItemView item1, IItemView itemViewInto, IGridView gridView, Action<EMergeResult, bool> callback)
        {
            callback?.Invoke(EMergeResult.NoMerge, false);
        }

        public EMergeResult TryMerge(ItemData item1, ItemData item2, out ItemData mergedItem, out bool oneIntoTwo)
        {
            oneIntoTwo = false;
            mergedItem = null;
            return EMergeResult.NoMerge;
        }

        public List<Vector2Int> GetCellsForPotentialMerge(List<ItemData> allItems, ItemData srcItem)
        {
            return new List<Vector2Int>();
        }

        public List<IItemView> MergeAllItemsPossible(List<IItemView> allItems, IGridView gridView) => null;

        public List<IItemView> SortAllItemsPossible(List<IItemView> allItems, IGridView gridView) => null;
        public void AddModifier(IMergeModifier mod)
        {
            throw new NotImplementedException();
        }

        public void RemoveModifier(IMergeModifier mod)
        {
            throw new NotImplementedException();
        }

        public void ClearAllModifiers()
        {
            throw new NotImplementedException();
        }

        public void SetGridView(IGridView gridView) { }

        public void SetMaxCount(int maxCount) { }

        public bool GetFreeAllowedCell(MergeGrid grid, ItemData itemData, out Vector2Int coordinates)
        {
            coordinates = default;
            return false;
        }

        public int GetFreeCellsCount() => 0;

        public List<ItemData> GetAllItems() => null;

        public List<ItemData> GetAllItemsInMergeArea() => null;

        public List<IItemView> GetAllItemViewsInMergeArea() => null;

        public List<T> GetItemsInActiveArea<T>(MiscUtils.Condition<T> condition) => null;

        public bool CanPutMoreIntoActiveZone() => true;

        public Vector2Int GetCoordinateForClosestCellInActiveZone(Vector2Int originalCell)
        {
            return default;
        }

         private void OnMergeInputEnd(MergePutResult res)
         {
             if (res == MergePutResult.PutToSameCell || res == MergePutResult.MissedCell)
                 return;
             RewritePlayerParty();
             if (_logActiveParty)
             {
                 var party = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().party;
                 var msg = $"Party: ";
                 for (var i = 0; i < HeroesConstants.PartySize; i++)
                     msg += $"{i} {party.heroesIds[i]}, ";
                 CLog.Log(msg);
             }
         }
    }
}