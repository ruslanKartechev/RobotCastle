using System;
using System.Collections;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using SleepDev.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.MainMenu
{
    public class TutorialUpgradeHero : TutorialBase, IItemValidator
    {
        public override void Begin(Action finishedCallback)
        {
            CLog.Log($"[TutorialUpgradeHero] Begin");
            _finishedCallback = finishedCallback;
            StartCoroutine(Working());
        }
        
        [SerializeField] private List<string> _messages1;
        [SerializeField] private List<string> _messages2;
        [SerializeField] private List<string> _messages3;
        [SerializeField] private float _handMoveTime = .6f;
        [SerializeField] private float _handMoveBetweenTime = 1.6f;
        [SerializeField] private Image _backGround;
        [Space(10)]
        [SerializeField] private Vector3 _clickOffsetBarracks;
        [SerializeField] private Vector3 _heroPosOffset;
        [SerializeField] private Vector3 _btnGrowthOffset;
        [SerializeField] private Vector3 _btnUpgradeOffset;
        [SerializeField] private Vector3 _btnReturnOffset;
        [SerializeField] private Vector3 _btnBattleOffset;
        [SerializeField] private Vector3 _btnOffsetChapters;
        [SerializeField] private Vector3 _btnOffsetDifficultyTier;
        [SerializeField] private Vector3 _btnOffsetPlay;
        private bool _upgradeCompleted;
        private SubParent _subParent = new();


        private IEnumerator Working()
        {
            _backGround.enabled = true;
            AddHeroesXpIfNecessary();
            var uiManager = ServiceLocator.Get<IUIManager>();
            var gate = uiManager.GetIfShown<GateTabUI>(UIConstants.UIGateTab);
            gate.battleBtn.SetInteractable(false);
            
            var tabs = uiManager.GetIfShown<MainMenuTabsUI>(UIConstants.UIMainMenuTabs);
            if (tabs == null)
            {
                CLog.LogRed("Error! Cannot get MainMenuTabsUI!");
            }
            _hand.Off();
            _subParent.Parent(tabs.ButtonsParent, _parent);
            
            
            tabs.gateBtn.SetInteractable(false);
            tabs.shopBtn.SetInteractable(false);
            var barracksBtn = tabs.barracksBtn;
            barracksBtn.SetInteractable(false);
            _textPrinter.Callback = StopWaiting;
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages1);

            _isWaiting = true;
            while (_isWaiting)
                yield return null;
            _textPrinter.Callback = BlankCallback;
            _subParent.Return();
            
            barracksBtn.SetInteractable(true);
            _hand.On();
            _hand.LoopClickingTracking(barracksBtn.transform, _clickOffsetBarracks, 0f);
            
            yield return WaitForBtn(barracksBtn);
            _backGround.enabled = false;
            _textPrinter.ShowMessages(_messages2);
            
            InitBarracksTutor();
            
            var barracksHeroView = ServiceLocator.Get<BarracksHeroView>();
            barracksHeroView.OnHeroShown += StopWaiting;
            _isWaiting = true;
            while (_isWaiting)
                yield return null;
            barracksHeroView.OnHeroShown -= StopWaiting;
            
            _hand.Off();
            _textPrinter.Off();
            yield return null;

            var heroPanel = uiManager.GetIfShown<BarracksHeroPanel>(UIConstants.UIBarracksHeroDescription);
            if (heroPanel == null)
            {
                CLog.LogRed($"Error! Cannot get BarracksHeroPanel");
            }
            heroPanel.btnBack.SetInteractable(false);
            _hand.On();
            _hand.LoopClickingTracking(heroPanel.btnGrowth.transform, _btnGrowthOffset, _handMoveTime);
                       
            yield return WaitForBtn(heroPanel.btnGrowth);

            _hand.Off(); 
            yield return null;
            var growthUI = uiManager.GetIfShown<HeroGrowthPanel>(UIConstants.UIHeroGrowth);
            if (growthUI == null)
            {
                CLog.LogRed("Error! Cannot get HeroGrowthPanel");
            }

            growthUI.BtnBack.interactable = false;
            var btnGrowth = growthUI.BtnGrowth;
            _hand.On(); 
            _hand.LoopClickingTracking(btnGrowth.transform, _btnUpgradeOffset, _handMoveTime);
            
            yield return WaitForBtn(btnGrowth);

            growthUI.BtnBack.interactable = true;
            _hand.MoveToAndLoopClicking(growthUI.BtnBack.transform.position, _handMoveTime);
            yield return WaitForBtn(growthUI.BtnBack);

            heroPanel.btnBack.SetInteractable(true);
            _hand.MoveToAndLoopClicking(heroPanel.btnBack.transform.position + _btnReturnOffset, _handMoveTime);
            heroPanel.btnGrowth.SetInteractable(false);
            yield return WaitForBtn(heroPanel.btnBack);
               
            _textPrinter.Show();
            _textPrinter.ShowMessages(_messages3);
            
            tabs.gateBtn.SetInteractable(true);
            tabs.barracksBtn.SetInteractable(false);
            barracksHeroView.OnHeroShown += OnHeroSelected;
            AnimateHandHeroes();
            yield return WaitForBtn(tabs.gateBtn);
            if (!_upgradeCompleted)
            {
                barracksHeroView.OnHeroShown -= OnHeroSelected;
                _textPrinter.Hide();
            }
            
            yield return EnterNextBattle();
            Complete();
        }

        private IEnumerator EnterNextBattle()
        {
            var uiManager = ServiceLocator.Get<IUIManager>();
            var gate = uiManager.GetIfShown<GateTabUI>(UIConstants.UIGateTab);
            gate.battleBtn.SetInteractable(true);
            _hand.On();
            _hand.MoveToAndLoopClicking(gate.battleBtn.transform.position + _btnBattleOffset, _handMoveTime);
            yield return WaitForBtn(gate.battleBtn);
            _hand.Off();
            yield return null;
            var gameModeUI = ServiceLocator.Get<IUIManager>().Show<GameModeSelectionUI>(UIConstants.UIGameModeSelection, () => { });
            gameModeUI.Show();
            gameModeUI.btnChapters2.SetInteractable(false);
            gameModeUI.closeBtn.SetInteractable(false);
            var chaptersBtn = gameModeUI.btnChapters1;
            var pos = chaptersBtn.transform.position + _btnOffsetChapters;
            _hand.On();
            _hand.MoveToAndLoopClicking(pos, _handMoveTime);
            yield return WaitForBtn(chaptersBtn);
            
            var chapterUI = gameModeUI.ChapterSelectionUI;
            chapterUI.DisableInputButPlayButton();
            chapterUI.HideAdditionalReward();

            chapterUI.PlayBtn.SetInteractable(false);
            chapterUI.TierUis[1].SetLocked(false);
            pos = chapterUI.TierUis[1].transform.position + _btnOffsetDifficultyTier;
            _hand.MoveToAndLoopClicking(pos, _handMoveTime);
            chapterUI.Inventory.OnNewPicked += OnPicked;
            _isWaiting = true;
            while (_isWaiting)
                yield return null;
            chapterUI.Inventory.OnNewPicked -= OnPicked;
            chapterUI.Inventory.enabled = false;
            chapterUI.PlayBtn.SetInteractable(true);
            
            pos = chapterUI.PlayBtn.transform.position + _btnOffsetPlay;
            _hand.MoveToAndLoopClicking(pos, _handMoveTime);
            
        }

        private void OnPicked(Item obj)
        {
            var gameModeUI = ServiceLocator.Get<IUIManager>().Show<GameModeSelectionUI>(UIConstants.UIGameModeSelection, () => { });
            var t1 = gameModeUI.ChapterSelectionUI.TierUis[1];
            if (obj == t1.item)
            {
                _isWaiting = false;
            }
        }

        private void Complete()
        {
            // _hand.Off();
            _textPrinter.Off();
            var barracksManager = ServiceLocator.Get<BarracksManager>();
            barracksManager.BarracksInput.Validator = null;
            _finishedCallback?.Invoke();
        }

        private void AnimateHandHeroes()
        {
            var barracksManager = ServiceLocator.Get<BarracksManager>();
            var grid = barracksManager.GridView.Grid;
            var length = grid.GetLength(0);
            var cam = Camera.main;
            var pos1 = grid[0, 0].ItemPoint.transform.position;
            pos1 += _heroPosOffset;
            pos1 = cam.WorldToScreenPoint(pos1);
            
            var pos2 = grid[length - 1, 0].ItemPoint.transform.position;
            pos2 += _heroPosOffset;
            pos2 = cam.WorldToScreenPoint(pos2);

            _hand.On();
            _hand.MoveBetween(pos1, pos2, _handMoveBetweenTime);
        }

        private void OnHeroSelected()
        {
            _upgradeCompleted = true;
            _hand.Off();
            _textPrinter.Hide();
            var barracksHeroView = ServiceLocator.Get<BarracksHeroView>();
            barracksHeroView.OnHeroShown -= OnHeroSelected;
        }

        private void InitBarracksTutor()
        {
            var barracksUI = ServiceLocator.Get<IUIManager>().GetIfShown<BarracksTabUI>(UIConstants.UIBarracksTab);
            if (barracksUI != null)
            {
                barracksUI.altarsBtn.SetInteractable(false);
                barracksUI.summonBtn.SetInteractable(false);
            }
            
            var barracksManager = ServiceLocator.Get<BarracksManager>();
            barracksManager.BarracksInput.Validator = this;

            var grid = barracksManager.GridView.Grid;
            var cell = grid[1, 0];
            var pos = cell.ItemPoint.transform.position;
            pos += _heroPosOffset;
            var screenPos = Camera.main.WorldToScreenPoint(pos);
            _hand.LoopClicking(screenPos);
        }
        

        private void BlankCallback() {}

        private void AddHeroesXpIfNecessary()
        {
            var heroes = DataHelpers.GetHeroesSave();
            var party = DataHelpers.GetPlayerParty();
            var saves = heroes.heroSaves.FindAll(t => party.heroesIds.Contains(t.id));
            foreach (var s in saves)
                CheckXp(s);

            void CheckXp(HeroSave save)
            {
                if (save.xp < save.xpForNext)
                {
                    save.xp = save.xpForNext;
                }
            }
        }

        public bool CheckIfValid(IItemView itemView) 
            => itemView.itemData.pivotY == 0;
    }
}