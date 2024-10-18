using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public partial class HeroGrowthPanel : MonoBehaviour, IScreenUI
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
                CLog.Log("[GrowthPanel] No grand xp operation");
            }
        }

        private void SetAppropriateMode()
        {
            var money = ServiceLocator.Get<GameMoney>().globalMoney.Val;
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
                    _currentOperation = new PurchaseUpgradeOperation(_heroSave, AnimateUpgradeXp, AnimateUpgradeStats, AnimateUpgradeLevel);
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
                                _currentOperation = new GrantXpOperation(_heroSave, itemPicked, AnimateUpgradeXp);
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
                    _currentOperation = new PurchaseUpgradeOperation(_heroSave, AnimateUpgradeXp, AnimateUpgradeStats, AnimateUpgradeLevel);
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
            CLog.Log($"[GrowthPanel] Item picked: {itemPicked.Id}");
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
                CLog.Log("[GrowthPanel] Next purchase Operation is not chosen");
                return;
            }
            CLog.Log($"[GrowthPanel] On Growth Btn. Mode: {_mode.ToString()}");
            var result = _currentOperation.Apply();
            if (result == 0)
            {
                SetStats();
                _heroLevelText.text = (_heroSave.level + 1).ToString();
                _xpText.text = $"{_heroSave.xp}/{_heroSave.xpForNext}";
                _xpFillImage.fillAmount = (float)(_heroSave.xp) / _heroSave.xpForNext;
            }
            SetAppropriateMode();
        }

        private void AnimateUpgradeStats()
        {
            SetStats(true);
        }

        private void AnimateUpgradeLevel()
        {
            _heroLevelText.text = (_heroSave.level + 1).ToString();
            _heroLevelText.transform.DOPunchScale(Vector3.one * .2f, .2f);
        }

        private void AnimateUpgradeXp(Vector2Int xpStart, Vector2Int xpEnd, float percentStart, float percentEnd)
        {
            // _xpText.text = $"{_heroSave.xp}/{_heroSave.xpForNext}";
            // _xpFillImage.fillAmount = (float)(_heroSave.xp) / _heroSave.xpForNext;
            if(_animating != null)
                StopCoroutine(_animating);
            _animating =StartCoroutine(XpBarAnimating(xpStart, xpEnd, percentStart, percentEnd));
        }

        private Coroutine _animating;

        private IEnumerator XpBarAnimating(Vector2Int xpStart, Vector2Int xpEnd, float percentStart, float percentEnd)
        {
            var time = .3f;
            var elapsed = 0f;
            var t = elapsed / time;
            while (t <= 1f)
            {
                var xp = Mathf.RoundToInt( Mathf.Lerp(xpStart.x, xpEnd.x, t) );
                var xpMax = Mathf.RoundToInt( Mathf.Lerp(xpStart.y, xpEnd.y, t) );
                _xpText.text = $"{xp}/{xpMax}";
                _xpFillImage.fillAmount = Mathf.Lerp(percentStart, percentEnd, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            _xpText.text = $"{xpEnd.x}/{xpEnd.y}";
            _xpFillImage.fillAmount = percentEnd;
        }

        private void Close()
        {
            gameObject.SetActive(false);
            ServiceLocator.Get<IUIManager>().OnClosed(UIConstants.UIHeroGrowth);
        }

        private void OnEnable()
        {
            _btnGrowth.onClick.AddListener(OnGrowth);
            _btnBack.onClick.AddListener(Close);
            _btnGrowthXp.onClick.AddListener(OnGrowth);
            _inventoryUI.OnNewPicked += OnNewInventoryItemPicked;
            _inventoryUI.OnNothingPicked += OnNothingInventoryItem;
        }

        private void OnDisable()
        {
            _btnGrowth.onClick.RemoveListener(OnGrowth);
            _btnBack.onClick.RemoveListener(Close);
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
    }
}