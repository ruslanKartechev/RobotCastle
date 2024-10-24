﻿using DG.Tweening;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.InvasionMode;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BattleMergeUI : MonoBehaviour, IScreenUI
    {
        public const float MainRectAnimateTime = .45f;
        
        public InvasionLevelsUI Level => _levelUI;
        public TroopSizePurchaseUI TroopSizePurchaseUI => _troopSizePurchaseUI;
        public IButtonInput BtnStart => _btnStart;
        public IButtonInput BtnStart2 => _btnStartTop;
        public PurchaseNewHeroButton BtnPurchaseHero => _btnPurchaseHero;
        public ReturnItemButton ReturnItemButton => _returnItemButton;
        public BattleDamageStatsUI StatsCollector => _damageStats;
        
        [SerializeField] private LevelMoneyUI _money;
        [SerializeField] private TroopsCountUI _troopsCount;
        [SerializeField] private MyButton _btnStart;
        [SerializeField] private PurchaseNewHeroButton _btnPurchaseHero;
        [SerializeField] private TroopSizePurchaseUI _troopSizePurchaseUI;
        [SerializeField] private BattleDamageStatsUI _damageStats;
        [SerializeField] private InvasionLevelsUI _levelUI;
        [SerializeField] private Vector2 _blockYPositions;
        [SerializeField] private RectTransform _mainRect;
        [SerializeField] private MyButton _btnStartTop;
        [SerializeField] private ReturnItemButton _returnItemButton;
        [SerializeField] private FadeInOutAnimator _startTopBtnAnimator;
        [SerializeField] private FadePopAnimator _winAnimator;
        [SerializeField] private FadePopAnimator _lostAnimator;
        [SerializeField] private FadePopAnimator _startedAnimator;

        private Battle _battle;
        private LevelData _levelData;
        
        public void Init(Battle battle, Chapter chapter)
        {
            _battle = battle;
            _levelData = chapter.levelData;
            _levelUI.LevelName = chapter.viewName;
            _levelUI.SetRewardForWave(chapter.levelData.levels[battle.roundIndex].reward);
            _levelUI.SetLevel(battle.roundIndex, chapter.levelData.levels, false);
            _money.Init(ServiceLocator.Get<GameMoney>().levelMoney);
            _money.DoReact(true);
        }

        public void AllowButtonsInput(bool allow)
        {
            BtnPurchaseHero.SetInteractable(allow);
            BtnStart.SetInteractable(allow);
            BtnStart2.SetInteractable(allow);
            TroopSizePurchaseUI.SetInteractable(allow);
        }
        
        public void UpdateForNextWave()
        {
            _levelUI.SetRewardForWave(_levelData.levels[_battle.roundIndex].reward);
            _levelUI.SetLevel(_battle.roundIndex, _levelData.levels, true);
        }

        public void Started()
        {
            _startedAnimator.Animate();
            SetMainAreaLowerPos();
        }

        public void Win()
        {
            _winAnimator.Animate();
        }

        public void Lost()
        {
            _lostAnimator.Animate();
        }
        
        public void SetMainAreaLowerPos()
        {
            _mainRect.DOKill();
            _mainRect.DOAnchorPosY(_blockYPositions.x, MainRectAnimateTime);
            if(_battle.State != BattleState.Going)
                _startTopBtnAnimator.FadeIn();
            else
                _startTopBtnAnimator.Off();
        }

        public void SetMainAreaUpPos()
        {
            _mainRect.DOKill();
            _mainRect.DOAnchorPosY(_blockYPositions.y, MainRectAnimateTime);
            _startTopBtnAnimator.FadeOut();
        }
        

        private void OnEnable()
        {
            var pos = _mainRect.anchoredPosition;
            pos.y = _blockYPositions.y;
            _mainRect.anchoredPosition = pos;
            _startTopBtnAnimator.Off();
            ServiceLocator.Bind<ITroopsCountView>(_troopsCount);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<ITroopsCountView>();
        }
    }
}
