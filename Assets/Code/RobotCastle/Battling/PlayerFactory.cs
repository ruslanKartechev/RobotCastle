using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using SleepDev.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class PlayerFactory : MonoBehaviour, IPlayerFactory
    {
        public event Action<PurchaseHeroResult> OnPurchase;

        public bool PurchaseAllowed { get; set; } = true; 
        public ReactiveInt NextCost => _costReact;
   
        public PurchaseHeroResult TryPurchaseItem(bool promptUser = true)
        {
            if (!PurchaseAllowed)
            {
                OnPurchase?.Invoke(PurchaseHeroResult.NotAllowed);
                return PurchaseHeroResult.NotAllowed;
            }
            var gameMoney = ServiceLocator.Get<GameMoney>();
            var money = gameMoney.levelMoney.Val;
            if (money < _costReact.Val)
            {
                CLog.Log($"[{nameof(PlayerFactory)}] Not enough money");
                if (promptUser)
                {
                    var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {});
                    ui.ShowNotEnoughMoney();
                }
                OnPurchase?.Invoke(PurchaseHeroResult.NotEnoughMoney);
                return PurchaseHeroResult.NotEnoughMoney;
            }

            var item = _itemsPicker.GetNext();
            var merge = ServiceLocator.Get<MergeManager>();
            var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
            var args = new SpawnArgs(item);
            factory.SpawnHeroOrItem(args, merge.GridView, merge.SectionsController, out var newItem);
            if (newItem != null)
            {
                money -= _cost;
                gameMoney.levelMoney.UpdateWithContext(money, (int)EMoneyChangeContext.AfterPurchase);
                merge.HighlightMergeOptions();
                var particles = ServiceLocator.Get<SimplePoolsManager>();
                var p = particles.GetOne("hero_spawn") as OneTimeParticles;
                p.Show(newItem.Transform.position);
                merge.Container.AddNewItem(newItem);

                foreach (var mod in _modifiers)
                    mod.OnNewItemSpawned(newItem);
                
                SoundManager.Inst.Play(_sound);
                OnPurchase?.Invoke(PurchaseHeroResult.Success);
                return PurchaseHeroResult.Success;
            }
            OnPurchase?.Invoke(PurchaseHeroResult.Error);
            return PurchaseHeroResult.Error;
        }

        public IItemView SpawnHeroOrItem(SpawnArgs args)
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
            else
                CLog.LogError("SPAWNED NEW ITEM NULL");
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
     
          
        [SerializeField] private int _cost = 3;
        [SerializeField] private SoundID _sound;
        private IPlayerSummonItemPicker _itemsPicker;
        private List<IPlayerItemSpawnModifier> _modifiers = new(10);
        private ReactiveInt _costReact;

        private void Awake()
        {
            _costReact = new ReactiveInt(_cost);
            _itemsPicker = gameObject.GetComponent<IPlayerSummonItemPicker>();
        }
   
    }
}