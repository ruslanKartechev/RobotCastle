using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using SleepDev;
using SleepDev.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class HeroGrowthPanel : MonoBehaviour, IScreenUI
    {
        private enum Mode {UpgradeForMoney, UpgradeFree, GrantXp}

        [SerializeField] private XpAddAnimation _addAnimation;
        [SerializeField] private TextMeshProUGUI _statTxtHealth;
        [SerializeField] private TextMeshProUGUI _statTxtAttack;
        [SerializeField] private TextMeshProUGUI _statTxtSpellPower;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI _statTxtHealth_2;
        [SerializeField] private TextMeshProUGUI _statTxtAttack_2;
        [SerializeField] private TextMeshProUGUI _statTxtSpellPower_2;
        [Space(10)] 
        [SerializeField] private TextMeshProUGUI _itemDescriptionHeader;
        [SerializeField] private TextMeshProUGUI _itemDescription;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI _heroLevelText;
        [SerializeField] private TextMeshProUGUI _xpText;
        [SerializeField] private Image _xpFillImage;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private TextMeshProUGUI _buttonTextXp;
        [SerializeField] private Button _btnGrowth;
        [SerializeField] private Button _btnGrowthXp;
        [SerializeField] private Button _btnBack;
        [Space(10)]
        [SerializeField] private InventoryController _inventoryUI;
        private HeroSave _heroSave;
        private PlayerInventory _inventory;
        private string _id;
        private Mode _mode;
        private IOperation _currentOperation;
        
        
        public void Show(string id)
        {
            gameObject.SetActive(true);
            _inventory = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
            _heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(id);
            SetStats();
            _heroLevelText.text = (_heroSave.level + 1).ToString(); // next, because it is an index
            _xpText.text = $"{_heroSave.xp}/{_heroSave.xpForNext}";
            _xpFillImage.fillAmount = (float)_heroSave.xp / _heroSave.xpForNext;

            _inventoryUI.Reset();
            _itemDescriptionHeader.text = "";
            _itemDescription.text = "";
            SetupInventory();
            SetMode(Mode.UpgradeForMoney);
            _currentOperation = new PurchaseUpgradeOperation(_heroSave);
        }

        private void SetStats(bool animated = false)
        {
            var lvl = _heroSave.level;
            var nextLvl = lvl + 1;
            var heroesDb = ServiceLocator.Get<HeroesDatabase>();
            var stats = heroesDb.GetHeroInfo(_heroSave.id).stats;
            var health1 = (int)HeroStatsManager.GetStatByLevel(stats.attack, lvl, 1);
            var atk1 = (int)HeroStatsManager.GetStatByLevel(stats.attack, lvl, 1);
            var sp1 = (int)HeroStatsManager.GetStatByLevel(stats.spellPower, lvl, 1);
            
            var health2 = (int)HeroStatsManager.GetStatByLevel(stats.attack, nextLvl, 1);
            var atk2 = (int)HeroStatsManager.GetStatByLevel(stats.attack, nextLvl, 1);
            var sp2 = (int)HeroStatsManager.GetStatByLevel(stats.spellPower, nextLvl, 1);

            _statTxtHealth.text = health1.ToString();
            _statTxtHealth_2.text = health2.ToString();
            _statTxtAttack.text = atk1.ToString();
            _statTxtAttack_2.text = atk2.ToString();
            _statTxtSpellPower.text = sp1.ToString();
            _statTxtSpellPower_2.text = sp2.ToString();
        }
        
        private void SetupInventory()
        {
            var notInInventory = new List<string>(5);
            foreach (var ui in _inventoryUI.AllItems)
            {
                var item = _inventory.items.Find(t => t.id == ui.Id);
                if (item != null)
                {
                    ui.SetCount(item.amount);
                    ui.IsAllowedToPick = item.amount > 0;
                }
                else
                {
                    notInInventory.Add(ui.Id);
                    ui.SetCount(0);
                }
            }
            foreach (var id in notInInventory)
                _inventory.items.Add(new InventoryItemData(id, 0));
        }

        private void ShowPossibleXpGain()
        {
            if (_currentOperation is GrantXpOperation xpOp)
            {
                var prog = (float)(xpOp.giveXpAmount + _heroSave.xp) / _heroSave.xpForNext;
                _addAnimation.Animate(prog);
            }
            else
            {
                CLog.Log("No grand xp operation");
            }
            
        }

        private void SetMode(Mode mode)
        {
            _mode = mode;
            switch (mode)
            {
                case Mode.GrantXp:
                    
                    _btnGrowth.gameObject.SetActive(false);
                    _btnGrowthXp.gameObject.SetActive(true);
                    _buttonTextXp.text = "Grant XP";
                    if (_heroSave.xp >= _heroSave.xpForNext)
                        _btnGrowthXp.interactable = false;
                    else
                        _btnGrowthXp.interactable = true;
                    break;
                case Mode.UpgradeFree:
                    _btnGrowth.gameObject.SetActive(false);
                    _btnGrowthXp.interactable = true;
                    _btnGrowthXp.gameObject.SetActive(true);
                    _buttonTextXp.text = "Upgrade Free";
                    break;
                case Mode.UpgradeForMoney:
                    _btnGrowthXp.gameObject.SetActive(false);
                    _btnGrowth.gameObject.SetActive(true);
                    var money = ServiceLocator.Get<GameMoney>().globalMoney;
                    var canUpgr = HeroesUpgradeManager.CanUpgrade(_heroSave, money, out var cost);
                    var color = "#FFFFFF";
                    switch (canUpgr)
                    {
                        case 0:
                            _btnGrowth.gameObject.SetActive(true);
                            _btnGrowth.interactable = true;
                            break;
                        case 1:
                            _btnGrowth.gameObject.SetActive(true);
                            _btnGrowth.interactable = false;         
                            break;
                        case 2:
                            _btnGrowth.gameObject.SetActive(true);
                            color = "#FF1111";
                            _btnGrowth.interactable = false;
                            break;
                        case 3:
                            CLog.Log($"Max level already reached");
                            _btnGrowth.gameObject.SetActive(false);
                            break;
                    }
                    _buttonText.text = $"Upgrade <color={color}>{cost}</color>";
                    break;
            }
        }

        private void OnNothingInventoryItem()
        {
            _currentOperation = new PurchaseUpgradeOperation(_heroSave);
            SetMode(Mode.UpgradeForMoney);
            _addAnimation.Stop();
            _itemDescriptionHeader.text = "";
            _itemDescription.text = "";
        }

        private void OnNewInventoryItemPicked(Item itemPicked)
        {
            CLog.Log($"Item picked: {itemPicked.Id}");
            if (_heroSave.xp >= _heroSave.xpForNext)
            {
                _currentOperation = new PurchaseUpgradeOperation(_heroSave);
                SetMode(Mode.UpgradeForMoney);
            }
            else
            {
                switch (itemPicked.Id)
                {
                    case "upgrade_cube":
                        _currentOperation = new InstantUpgradeOperation(_heroSave, itemPicked);
                        SetMode(Mode.UpgradeFree);
                        break;
                    case "book" or "king_medal" or "hero_medal":
                        _currentOperation = new GrantXpOperation(_heroSave, itemPicked);
                        SetMode(Mode.GrantXp);
                        ShowPossibleXpGain();
                        break;
                }
            }
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            var info = db.GetDescription(itemPicked.Id);
            _itemDescriptionHeader.text = info.parts[0];
            _itemDescription.text = info.parts[1];
        }

        private void OnGrowth()
        {
            CLog.Log($"On Growth Btn. Mode: {_mode.ToString()}");
            _currentOperation.Apply();
            _addAnimation.Stop();
            switch (_mode)
            {
                case Mode.GrantXp:
                    SetStats();
                    if (_heroSave.xp >= _heroSave.xpForNext)
                    {
                        _currentOperation = new PurchaseUpgradeOperation(_heroSave);
                        SetMode(Mode.UpgradeForMoney);
                    }
                    else
                    {
                        if (_inventoryUI.PickedItem.GetCount() > 0)
                        {
                            SetMode(Mode.GrantXp);
                            ShowPossibleXpGain();
                        }
                        else
                        {
                            SetMode(Mode.UpgradeForMoney);
                        }       
                    }
                    break;
                case Mode.UpgradeFree:
                    if (_inventoryUI.PickedItem.GetCount() > 0)
                    {
                        SetMode(Mode.GrantXp);
                        ShowPossibleXpGain();
                    }
                    else
                    {
                        SetMode(Mode.UpgradeForMoney);
                    }
                    break;
                case Mode.UpgradeForMoney:
                    SetStats(true);
                    if (_heroSave.xp < _heroSave.xpForNext && _inventoryUI.PickedItem != null)
                    {
                        _currentOperation = new GrantXpOperation(_heroSave, _inventoryUI.PickedItem);
                        SetMode(Mode.GrantXp);
                        ShowPossibleXpGain();
                    }
                    else
                    {
                        SetMode(Mode.UpgradeForMoney);
                        _currentOperation = new PurchaseUpgradeOperation(_heroSave);
                    }
                    break;
            }
            _heroLevelText.text = (_heroSave.level + 1).ToString();
            _xpText.text = $"{_heroSave.xp}/{_heroSave.xpForNext}";
            _xpFillImage.fillAmount = (float)(_heroSave.xp) / _heroSave.xpForNext;
        }

        private void OnReturnBtn()
        {
            CLog.Log($"On Return Btn");
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _btnGrowth.onClick.AddListener(OnGrowth);
            _btnBack.onClick.AddListener(OnReturnBtn);
            _btnGrowthXp.onClick.AddListener(OnGrowth);
            _inventoryUI.OnNewPicked += OnNewInventoryItemPicked;
            _inventoryUI.OnNothingPicked += OnNothingInventoryItem;
        }

        private void OnDisable()
        {
            _btnGrowth.onClick.RemoveListener(OnGrowth);
            _btnBack.onClick.RemoveListener(OnReturnBtn);
            _btnGrowthXp.onClick.RemoveListener(OnGrowth);
            _inventoryUI.OnNewPicked -= OnNewInventoryItemPicked;
            _inventoryUI.OnNothingPicked -= OnNothingInventoryItem;
        }

        
        private interface IOperation {
            void Apply();
        }

        private class InstantUpgradeOperation : IOperation
        {
            public Item item;
            public HeroSave heroSave;

            public InstantUpgradeOperation(HeroSave heroSave, Item item)
            {
                this.heroSave = heroSave;
                this.item = item;
            }
            
            public void Apply()
            {
                HeroesUpgradeManager.UpgradeNoCharge(heroSave);
                var count = item.GetCount();
                item.SetCount(count-1);
            }
        }
        
        private class PurchaseUpgradeOperation : IOperation
        {
            public HeroSave heroSave;

            public PurchaseUpgradeOperation(HeroSave heroSave)
            {
                this.heroSave = heroSave;
            }
            
            public void Apply()
            {
                HeroesUpgradeManager.UpgradeHero(heroSave);
            }
        }

        private class GrantXpOperation : IOperation
        {
            public HeroSave heroSave;
            public Item itemUI;
            public float needXpAmount;
            public int needItemsCount;
            public int giveXpAmount;
            public int consumeItemsCount;
            
            
            public GrantXpOperation(HeroSave heroSave, Item itemUI)
            {
                this.itemUI = itemUI;
                this.heroSave = heroSave;
                var xpdb = ServiceLocator.Get<XpDatabase>();
                var xpPerItem = xpdb.xpGrantedByItem[itemUI.Id];
                needXpAmount = (float)(this.heroSave.xpForNext - this.heroSave.xp);
                var totalItemsCount = itemUI.GetCount();
                var necessaryCount = Mathf.CeilToInt(needXpAmount / xpPerItem);
                if (totalItemsCount >= necessaryCount)
                {
                    giveXpAmount = Mathf.CeilToInt(needXpAmount);
                    consumeItemsCount = necessaryCount;
                }
                else
                {
                    consumeItemsCount = totalItemsCount;
                    giveXpAmount = Mathf.CeilToInt(totalItemsCount * xpPerItem);
                }
            }

            public void Apply()
            {
                heroSave.xp += giveXpAmount;
                var count = itemUI.GetCount();
                itemUI.SetCount(count - consumeItemsCount);
            }
        }
    }
}