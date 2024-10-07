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
            SetAppropriateMode();
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

        private void SetAppropriateMode()
        {
            var money = ServiceLocator.Get<GameMoney>().globalMoney;
            var canUpgr = HeroesManager.CanUpgrade(_heroSave, money, out var cost);
            switch (canUpgr)
            {
                case 0: // can
                    _btnGrowth.gameObject.SetActive(true);
                    _btnGrowthXp.gameObject.SetActive(false);
                    _buttonText.text = $"Upgrade <color=#FFFFFF>{cost}</color>";
                    _btnGrowth.interactable = true;
                    _mode = Mode.UpgradeForMoney;
                    _addAnimation.Stop();
                    _currentOperation = new PurchaseUpgradeOperation(_heroSave);
                    break;
                case 1: // no xp
                    _btnGrowth.gameObject.SetActive(false);
                    _btnGrowthXp.gameObject.SetActive(true);
                    var itemPicked = _inventoryUI.PickedItem;
                    if (itemPicked != null && itemPicked.GetCount() > 0)
                    {
                        _btnGrowthXp.interactable = true;
                        switch (itemPicked.Id)
                        {
                            case "upgrade_cube":
                                _buttonTextXp.text = "Instant Upgrade";
                                _mode = Mode.UpgradeFree;
                                _currentOperation = new InstantUpgradeOperation(_heroSave, itemPicked);
                                break;
                            case "book" or "king_medal" or "hero_medal":
                                _buttonTextXp.text = "Grant XP";
                                _mode = Mode.GrantXp;
                                _currentOperation = new GrantXpOperation(_heroSave, itemPicked);
                                ShowPossibleXpGain();
                                break;
                        }
                    }
                    else
                    {
                        _buttonTextXp.text = "Grant XP";
                        _btnGrowthXp.interactable = false;
                        _addAnimation.Stop();
                        _mode = Mode.GrantXp;
                    }
                    break;
                case 2: // no money
                    _btnGrowth.gameObject.SetActive(true);
                    _btnGrowthXp.gameObject.SetActive(false);
                    _buttonText.text = $"Upgrade <color=#FF1111>{cost}</color>";
                    _btnGrowth.interactable = false;
                    _mode = Mode.UpgradeForMoney;
                    _addAnimation.Stop();
                    _currentOperation = new PurchaseUpgradeOperation(_heroSave);
                    break;
                case 3:
                    _btnGrowth.gameObject.SetActive(false);
                    _btnGrowthXp.gameObject.SetActive(false);
                    _mode = Mode.UpgradeForMoney;
                    _addAnimation.Stop();
                    _currentOperation = null;
                    break;
            }
        }
        
        private void OnNothingInventoryItem()
        {
            _itemDescriptionHeader.text = "";
            _itemDescription.text = "";
            SetAppropriateMode();
        }

        private void OnNewInventoryItemPicked(Item itemPicked)
        {
            CLog.Log($"Item picked: {itemPicked.Id}");
            SetAppropriateMode();
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            var info = db.GetDescription(itemPicked.Id);
            _itemDescriptionHeader.text = info.parts[0];
            _itemDescription.text = info.parts[1];
        }

        private void OnGrowth()
        {
            if (_currentOperation == null)
            {
                CLog.Log("Next purcahse Operation is not chosen");
                return;
            }
            CLog.Log($"On Growth Btn. Mode: {_mode.ToString()}");
            var result = _currentOperation.Apply();
            if (result == 0 && _mode == Mode.UpgradeForMoney)
            {
                SetStats();
                _heroLevelText.text = (_heroSave.level + 1).ToString();
                _xpText.text = $"{_heroSave.xp}/{_heroSave.xpForNext}";
                _xpFillImage.fillAmount = (float)(_heroSave.xp) / _heroSave.xpForNext;
            }
            SetAppropriateMode();
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
            /// <summary>
            /// </summary>
            /// <returns>int ExitCode -> 0 is success by default </returns>
            int Apply();
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
            
            public int Apply()
            {
                HeroesManager.UpgradeNoCharge(heroSave);
                var count = item.GetCount();
                item.SetCount(count-1);
                return 0;
            }
        }
        
        private class PurchaseUpgradeOperation : IOperation
        {
            public HeroSave heroSave;

            public PurchaseUpgradeOperation(HeroSave heroSave)
            {
                this.heroSave = heroSave;
            }
            
            public int Apply()
            {
                return HeroesManager.UpgradeHero(heroSave);
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

            public int Apply()
            {
                heroSave.xp += giveXpAmount;
                var count = itemUI.GetCount();
                itemUI.SetCount(count - consumeItemsCount);
                return 0;
            }
        }
    }
}