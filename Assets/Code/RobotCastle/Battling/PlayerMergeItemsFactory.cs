using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class PlayerMergeItemsFactory : MonoBehaviour, IPlayerMergeItemsFactory
    {
        public int NextCost => _cost;
        
        [SerializeField] private int _cost = 3;
        private IPlayerSummonItemPicker _itemsPicker;
        private List<IPlayerItemSpawnModifier> _modifiers = new(10);

        private void Awake()
        {
            _itemsPicker = gameObject.GetComponent<IPlayerSummonItemPicker>();
        }

        public void TryPurchaseItem(bool promptUser = true)
        {
            var gameMoney = ServiceLocator.Get<GameMoney>();
            var money = gameMoney.levelMoney;
            if (money < _cost)
            {
                CLog.Log($"[{nameof(PlayerMergeItemsFactory)}] Not enough money");
                if (promptUser)
                {
                    var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {});
                    ui.ShowNotEnoughMoney();
                }
                return;
            }

            var item = _itemsPicker.GetNext();
            var merge = ServiceLocator.Get<MergeManager>();
            var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
            var args = new SpawnMergeItemArgs(item);
            factory.SpawnHeroOrItem(args, merge.GridView, merge.SectionsController, out var newItem);
            if (newItem != null)
            {
                money -= _cost;
                gameMoney.levelMoney = money;
                merge.HighlightMergeOptions();
                var particles = ServiceLocator.Get<SimplePoolsManager>();
                var p = particles.GetOne("hero_spawn") as OneTimeParticles;
                p.Show(newItem.Transform.position);
                merge.Container.AddNewItem(newItem);

                foreach (var mod in _modifiers)
                    mod.OnNewItemSpawned(newItem);
            }
        }

        public IItemView SpawnHeroOrItem(SpawnMergeItemArgs args)
        {
            var merge = ServiceLocator.Get<MergeManager>();
            var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
            factory.SpawnHeroOrItem(args, merge.GridView, merge.SectionsController, out var newItem);
            if (newItem != null)
            {
                merge.HighlightMergeOptions();
                var particles = ServiceLocator.Get<SimplePoolsManager>();
                var p = particles.GetOne("hero_spawn") as OneTimeParticles;
                p.Show(newItem.Transform.position);
                merge.Container.AddNewItem(newItem);
                
                foreach (var mod in _modifiers)
                    mod.OnNewItemSpawned(newItem);
            }
            return newItem;
        }
        
        public void AddModifier(IPlayerItemSpawnModifier mod)
        {
            _modifiers.Add(mod);   
        }

        public void RemoveModifier(IPlayerItemSpawnModifier mod)
        {
            _modifiers.Remove(mod);   
        }

        public void ClearAllModifiers()
        {
            _modifiers.Clear();
        }
        
    }
}