using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
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
            set
            {
                _isCorruption = value;
                _data.corruption = value;
            }
        }
        
        public Action ReturnCallback { get; set; }
        
        [SerializeField] private bool _isCorruption;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private InventoryController _inventory;
        [SerializeField] private TextMeshProUGUI _txtLevel;
        [SerializeField] private TextMeshProUGUI _txtName;
        [SerializeField] private TextMeshProUGUI _enemiesTotalPower;
        [SerializeField] private List<DifficultyTierUI> _tierUis;
        [SerializeField] private ChapterIconUI _chapterIcon;
        [Space(10)]
        [SerializeField] private MyButton _nextChapterBtn;
        [SerializeField] private MyButton _prevChapterBtn;
        [Space(10)]
        [SerializeField] private MyButton _playBtn;
        [SerializeField] private MyButton _returnBtn;
        [Space(10)]        
        [SerializeField] private HeroesPartyUI _partyUI;
        [SerializeField] private AdditionalRewardUI _additionalReward;
        [SerializeField] private DifficultyTierDescriptionUI _difficultyTierUI;
        [SerializeField] private ChapterRewardsUI _rewardsUI;
        [SerializeField] private SoundID _selectedChapterSound;
        [Header("EDITOR REVIEW")]
        [SerializeField] private ChapterSelectionData _data;
        private InvasionMode.ProgressionDataBase _chaptersDb;
        private SaveInvasionProgression _progresSave;
        private bool _isLoading;

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
            _progresSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().progression;
            _progresSave.SetFirstTierUnlockedIfChapterUnlocked();
            for (var i = 0; i < 5; i++)
                _inventory.AllItems[i].Id = i.ToString();
            if (_data == null)
            {
                _data = new ChapterSelectionData();
            }

            _data.chapterIndex = _progresSave.chapterIndex;
            _data.tierIndex = _progresSave.tierLevel;
            
            for (var i = 0; i < _inventory.AllItems.Count; i++)
            {
                var it = _inventory.AllItems[i];
                it.NumberId = i;
            }

            _additionalReward.SelectionData = _data;
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
            DataHelpers.GetPlayerData().chapterSelectionData = _data;
            _isLoading = false;
        }

        public void Play()
        {
            if (_isLoading) return;
            if (_progresSave.chapters[_data.chapterIndex].unlocked == false)
            {
                CLog.Log($"Chapter not unlocked, won't play");
                return;
            }
            var tierUnlocked = GetChapterSave().tierData[_data.tierIndex].unlocked;
            if (!tierUnlocked)
            {
                CLog.Log("Tier not unlocked, won't play");
                return;
            }
            _isLoading = true;
            CLog.LogGreen($"Playing chapter {_data.chapterIndex}, tier {_data.tierIndex}");
            _data.corruption = _isCorruption;
            DataHelpers.GetPlayerData().chapterSelectionData = _data;
            SoundManager.Inst.Play(_selectedChapterSound, false);
            ScreenDarkening.Animate(() =>
            {
                ServiceLocator.Get<SceneLoader>().LoadBattleScene();
            }, null);
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
            var nextIndex = _data.chapterIndex + 1;
            if (nextIndex >= _chaptersDb.chapters.Count)
                return;
            _data.chapterIndex = nextIndex;
            UpdateChapter(2);
        }

        public void PrevChapter()
        {
            if (_isLoading) return;
            var prevIndex = _data.chapterIndex - 1;
            if (prevIndex < 0)
                return;
            _data.chapterIndex = prevIndex;
            UpdateChapter(1);
        }
        
        private void OnNewTierPicked(Item item)
        {
            _data.tierIndex = item.NumberId;
            UpdateTier();
        }
        
        /// <param name="nextOrPrev">0 - none, 1 - prev, 2 - next </param>
        private void UpdateChapter(int nextOrPrev)
        {
            var chapterInd = _data.chapterIndex;
            // CLog.Log($"Updating chapter. Index: {chapterInd}. Unlocked: {_progresSave.chapters[chapterInd].unlocked}");
            var chapter = GetChapter();
            _txtLevel.text = $"Chapter {_data.chapterIndex + 1}";
            var title = _isCorruption ? $"<color=#FF1111>Corruption</color> " : "";
            title += chapter.viewName;
            _txtName.text = title;

            var viewDb = ServiceLocator.Get<ViewDataBase>();
            var iconPath = _isCorruption ? viewDb.LocationIconsCorruption[_data.chapterIndex] : viewDb.LocationIcons[_data.chapterIndex];
            var icon = Resources.Load<Sprite>(iconPath);
            var chapterSave = GetChapterSave();
            var unlocked = chapterSave.unlocked;
            if (!unlocked)
            {
                _chapterIcon.SetUnlockRequirement(_data.chapterIndex - 1, chapter.prevTierRequired);
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
                _inventory.SetItemPicked(_data.tierIndex);
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
        }

        private void UpdateTierRewardOnly()
        {
            var tierInd = _data.tierIndex;
            var chapter = GetChapter();
            var multiplier = chapter.tiers[tierInd].multiplier;
            _data.basicRewardMultiplier = multiplier;
            var goldReward = Mathf.RoundToInt(chapter.moneyGoldReward * multiplier);
            var tierSave = GetChapterSave().tierData[tierInd];
            _playBtn.SetInteractable(tierSave.unlocked);
            _rewardsUI.SetRewards(goldReward, chapter.tiers[tierInd].additionalRewards);
            _enemiesTotalPower.text = chapter.tiers[tierInd].totalPower.ToString();
        }

        private void UpdateTier()
        {
            var tierInd = _data.tierIndex;
            var chapter = GetChapter();
            var multiplier = chapter.tiers[tierInd].multiplier;
            _data.basicRewardMultiplier = multiplier;
            var goldReward = Mathf.RoundToInt(chapter.moneyGoldReward * multiplier);
            var tierSave = GetChapterSave().tierData[tierInd];
            _playBtn.SetInteractable(tierSave.unlocked);
            _rewardsUI.SetRewards(goldReward, chapter.tiers[tierInd].additionalRewards);

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
                return _chaptersDb.corruptionChapters[_data.chapterIndex];
            else
                return _chaptersDb.chapters[_data.chapterIndex];
        }

        private SaveInvasionProgression.ChapterData GetChapterSave()
        {
            if (_isCorruption)
                return _progresSave.chaptersCorruption[_data.chapterIndex];
            else
                return _progresSave.chapters[_data.chapterIndex];
        }
        
        private void CorrectTierIndex()
        {
            var currentIndex = _data.tierIndex;
            var tiers = GetChapterSave().tierData;
            // CLog.Log($"[CorrectTierIndex]. Chapter Ind: {_data.chapterIndex}. TierInd  {currentIndex}. {tiers[currentIndex].unlocked}");
            if (tiers[currentIndex].unlocked == false)
            {
                for (var ind = currentIndex; ind >= 0; ind--)
                {
                    if (tiers[ind].unlocked)
                    {
                        _data.tierIndex = ind;
                        return;
                    }
                }
            }
            else 
                return;
            CLog.LogError("Error counting back");
            _data.tierIndex = 0;
        }
    }
}