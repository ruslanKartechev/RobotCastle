﻿using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.MainMenu;
using RobotCastle.Saving;
using SleepDev;
using SleepDev.Inventory;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class InvasionChapterSelectionUI : MonoBehaviour
    {
        public bool IsCorruption
        {
            get => _isCorruption;
            set => _isCorruption = value;
        }
        
        public Action ReturnCallback { get; set; }
        public MyButton PlayBtn => _playBtn;
        public List<DifficultyTierUI> TierUis => _tierUis;
        public InventoryController Inventory => _inventory;
        
        [SerializeField] private bool _isCorruption;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private InventoryController _inventory;
        [SerializeField] private TextMeshProUGUI _txtLevel;
        [SerializeField] private TextMeshProUGUI _txtName;
        [SerializeField] private TextMeshProUGUI _enemiesTotalPower;
        [SerializeField] private List<DifficultyTierUI> _tierUis;
        [SerializeField] private ChapterIconUI _chapterIcon;
        [Space(10)]
        [SerializeField] private ChapterSelectButton _nextChapterBtn;
        [SerializeField] private ChapterSelectButton _prevChapterBtn;
        [Space(10)]
        [SerializeField] private MyButton _playBtn;
        [SerializeField] private MyButton _returnBtn;
        [Space(10)]        
        [SerializeField] private HeroesPartyUI _partyUI;
        [SerializeField] private AdditionalRewardUI _additionalReward;
        [SerializeField] private DifficultyTierDescriptionUI _difficultyTierUI;
        [SerializeField] private ChapterRewardsUI _rewardsUI;
        [SerializeField] private SoundID _selectedChapterSound;
        private ChapterSelectionData _selectionData;
        private InvasionMode.ProgressionDataBase _chaptersDb;
        private SaveInvasionProgression _save;
        private bool _isLoading;
        private EnergyOffer _energyOffer;

        public void Off()
        {
            _canvas.enabled = false;
        }

        public void Show(Action callbackReturn)
        {
            ReturnCallback = callbackReturn;
            _canvas.enabled = true;
            gameObject.SetActive(true);
            _chaptersDb = ServiceLocator.Get<InvasionMode.ProgressionDataBase>();
            _save = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().progression;
            var playerData = DataHelpers.GetPlayerData();
            _save.SetFirstTierUnlockedIfChapterUnlocked();
            for (var i = 0; i < 5; i++)
                _inventory.AllItems[i].Id = i.ToString();
            _selectionData = playerData.chapterSelectionData;
            if (_selectionData == null)
            {
                _selectionData = new ChapterSelectionData();
                playerData.chapterSelectionData = _selectionData;
            }
            _selectionData.corruption = IsCorruption;
            for (var i = 0; i < _inventory.AllItems.Count; i++)
            {
                var it = _inventory.AllItems[i];
                it.NumberId = i;
            }

            _additionalReward.SelectionData = _selectionData;
            _additionalReward.On(); // CHANGE THIS !!
            _additionalReward.Init();
            _partyUI.SetupHeroes();
            _partyUI.On();
            UpdateChapter(0);
            _inventory.OnNewPicked -= OnNewTierPicked;
            _inventory.OnNewPicked += OnNewTierPicked;
            _inventory.AllowPickNothing = false;
            _nextChapterBtn.AddMainCallback(NextChapter);
            _prevChapterBtn.AddMainCallback(PrevChapter);
            _playBtn.AddMainCallback(Play);
            _returnBtn.AddMainCallback(Return);
            _isLoading = false;
        }
        
        public void Play()
        {
            if (_isLoading) return;
            if(!CheckIfUnlocked())
                return;
            if (!CheckEnergy())
                return;
            StartLoadingLevel();
        }

        public void Return()
        {
            if (_isLoading) return;
            _partyUI.Off();
            _canvas.enabled = false;
            ReturnCallback?.Invoke();
        }

        public void NextChapter()
        {
            if (_isLoading) return;
            var nextIndex = _selectionData.chapterIndex + 1;
            if (nextIndex >= _chaptersDb.chapters.Count)
                return;
            _selectionData.chapterIndex = nextIndex;
            UpdateChapter(2);
        }

        public void PrevChapter()
        {
            if (_isLoading) return;
            var prevIndex = _selectionData.chapterIndex - 1;
            if (prevIndex < 0)
                return;
            _selectionData.chapterIndex = prevIndex;
            UpdateChapter(1);
        }

        public void DisableInputButPlayButton()
        {
            _returnBtn.SetInteractable(false);
            _nextChapterBtn.SetInteractable(false);
            _prevChapterBtn.SetInteractable(false);
            
        }

        public void HideAdditionalReward()
        {
            _additionalReward.Off();
        }
        
        private bool CheckEnergy(bool makeOfferIfNotEnough = true)
        {
            var ss = ServiceLocator.Get<PlayerEnergyManager>();
            var cost = _selectionData.totalEnergyCost;
            if (ss.GetCurrent() < cost)
            {
                if (!makeOfferIfNotEnough)
                    return false;
                CLog.LogBlue($"Not enough energy: cost {cost}, amount: {ss.GetCurrent()}");
                if (_energyOffer == null)
                    _energyOffer = new EnergyOffer();
                _energyOffer.MakeOffer(OnEnergyOfferCallback);
                return false;
            }
            return true;
        }

        private void OnEnergyOfferCallback()
        {
            CLog.Log($"[OnEnergyOfferCallback]");
            if (CheckEnergy(false))
                StartLoadingLevel(false);
        }

        private bool CheckIfUnlocked()
        {
            if (_save.chapters[_selectionData.chapterIndex].unlocked == false)
            {
                CLog.Log($"Chapter not unlocked, won't play");
                return false;
            }
            var tierUnlocked = GetChapterSave().tierData[_selectionData.tierIndex].unlocked;
            if (!tierUnlocked)
            {
                CLog.Log("Tier not unlocked, won't play");
                return false;
            }
            return true;
        }

        private void StartLoadingLevel(bool subtractEnergy = true)
        {
            _isLoading = true;
            CLog.LogGreen($"Playing chapter {_selectionData.chapterIndex}, tier {_selectionData.tierIndex}");
            _selectionData.corruption = _isCorruption;
            DataHelpers.GetPlayerData().chapterSelectionData = _selectionData;
            SoundManager.Inst.Play(_selectedChapterSound, false);
            if(subtractEnergy)
                ServiceLocator.Get<PlayerEnergyManager>().Subtract(_selectionData.totalEnergyCost);
            ScreenDarkening.Animate(() =>
            {
                ServiceLocator.Get<SceneLoader>().LoadBattleScene();
            }, null);
        }
        
        
        private void OnNewTierPicked(Item item)
        {
            _selectionData.tierIndex = item.NumberId;
            UpdateTier();
        }
        
        /// <param name="nextOrPrev">0 - none, 1 - prev, 2 - next </param>
        private void UpdateChapter(int nextOrPrev)
        {
            // CLog.Log($"Updating chapter. Index: {chapterInd}. Unlocked: {_progresSave.chapters[chapterInd].unlocked}");
            var chapter = GetChapter();
            _txtLevel.text = $"Chapter {_selectionData.chapterIndex + 1}";
            var title = _isCorruption ? $"<color=#FF1111>Corruption</color> " : "";
            title += chapter.viewName;
            _txtName.text = title;

            var viewDb = ServiceLocator.Get<ViewDataBase>();
            var iconPath = _isCorruption ? viewDb.LocationIconsCorruption[_selectionData.chapterIndex] : viewDb.LocationIcons[_selectionData.chapterIndex];
            var icon = Resources.Load<Sprite>(iconPath);
            var chapterSave = GetChapterSave();
            var unlocked = chapterSave.unlocked;
            if (!unlocked)
            {
                _difficultyTierUI.Hide(); 
                _chapterIcon.SetUnlockRequirement(_selectionData.chapterIndex - 1, chapter.prevTierRequired);
            }
            switch (nextOrPrev)
            {
                case 1:
                    _chapterIcon.AnimatePrev(icon, unlocked);
                    break;
                case 2:
                    _chapterIcon.AnimateNext(icon, unlocked);
                    break;
                default:
                    _chapterIcon.SetIcon(icon, unlocked);
                    break;
            }
            
            var tiersCount = chapter.tiers.Count;
            for (var i = 0; i < tiersCount; i++)
                _tierUis[i].SetPercentMultiplier(chapter.tiers[i].multiplier);
            if (unlocked)
            {
                for (var i = 0; i < tiersCount; i++)
                {
                    var tierUnlocked = chapterSave.tierData[i].unlocked;
                    // _tierUis[i].SetInteractable(tierUnlocked);
                    _tierUis[i].SetLocked(!tierUnlocked);
                }
                CorrectTierIndex();
                _inventory.SetItemPicked(_selectionData.tierIndex);
                UpdateTier();
            }
            else
            {
                UpdateTierRewardOnly();
                _inventory.SetNoItem();
                for (var i = 0; i < tiersCount; i++)
                {
                    // _tierUis[i].SetInteractable(false);
                    _tierUis[i].SetLocked(true);
                }
            }

            SetNextButtonState();
        }

        private void SetNextButtonState()
        {
            var chapterInd = _selectionData.chapterIndex;
            var allChapters = _isCorruption ? _chaptersDb.corruptionChapters : _chaptersDb.chapters;
            var count = allChapters.Count;
            if(chapterInd >= count - 1)
                _nextChapterBtn.SetState(false);
            else
            {
                var nextInd = chapterInd + 1;
                var saves = _isCorruption ? _save.chaptersCorruption : _save.chapters;
                if (saves[nextInd].unlocked && saves[nextInd].tierData[0].unlocked &&
                    !saves[nextInd].tierData[0].completed)
                {
                    _nextChapterBtn.SetState(true);
                    return;
                }
            }
            _nextChapterBtn.SetState(false);
        }

        private void UpdateTierRewardOnly()
        {
            var tierInd = _selectionData.tierIndex;
            var chapter = GetChapter();
            var multiplier = chapter.tiers[tierInd].multiplier;
            _selectionData.tierRewardMultiplier = multiplier;
            _selectionData.goldReward = (int)chapter.moneyGoldReward;
            var goldReward = Mathf.RoundToInt(chapter.moneyGoldReward * multiplier);
            var tierSave = GetChapterSave().tierData[tierInd];
            _playBtn.SetInteractable(tierSave.unlocked);
            if(!tierSave.completed)
                _rewardsUI.SetRewards(goldReward, chapter.tiers[tierInd].additionalRewards);
            else
                _rewardsUI.SetRewards(goldReward, null);
            _enemiesTotalPower.text = chapter.tiers[tierInd].totalPower.ToString();
        }

        private void UpdateTier()
        {
            var tierInd = _selectionData.tierIndex;
            var chapter = GetChapter();
            var multiplier = chapter.tiers[tierInd].multiplier;
            _selectionData.tierRewardMultiplier = multiplier;
            _selectionData.goldReward = (int)chapter.moneyGoldReward;
            var goldReward = Mathf.RoundToInt(chapter.moneyGoldReward * multiplier);
            var tierSave = GetChapterSave().tierData[tierInd];
            _playBtn.SetInteractable(tierSave.unlocked);

            if(!tierSave.completed)
                _rewardsUI.SetRewards(goldReward, chapter.tiers[tierInd].additionalRewards);
            else
                _rewardsUI.SetRewards(goldReward, null);
            
            _additionalReward.UpdateDataView();
            if (tierInd == 0)
            {
                _difficultyTierUI.Hide(false);   
            }
            else if (tierInd > 0)
            {
                var difficultyConfig = ServiceLocator.Get<DifficultyTiersDatabase>().tiersConfig[tierInd];
                _difficultyTierUI.Show(difficultyConfig.textDescription, true);
            }

            _enemiesTotalPower.text = chapter.tiers[tierInd].totalPower.ToString();
        }

        private Chapter GetChapter()
        {
            if (_isCorruption)
                return _chaptersDb.corruptionChapters[_selectionData.chapterIndex];
            return _chaptersDb.chapters[_selectionData.chapterIndex];
        }

        private SaveInvasionProgression.ChapterData GetChapterSave()
        {
            if (_isCorruption)
                return _save.chaptersCorruption[_selectionData.chapterIndex];
            return _save.chapters[_selectionData.chapterIndex];
        }
        
        private void CorrectTierIndex()
        {
            var currentIndex = _selectionData.tierIndex;
            var tiers = GetChapterSave().tierData;
            // CLog.Log($"[CorrectTierIndex]. Chapter Ind: {_data.chapterIndex}. TierInd  {currentIndex}. {tiers[currentIndex].unlocked}");
            if (tiers[currentIndex].unlocked == false)
            {
                for (var ind = currentIndex; ind >= 0; ind--)
                {
                    if (tiers[ind].unlocked)
                    {
                        _selectionData.tierIndex = ind;
                        return;
                    }
                }
            }
            else 
                return;
            CLog.LogError("Error counting back");
            _selectionData.tierIndex = 0;
        }
    }
}