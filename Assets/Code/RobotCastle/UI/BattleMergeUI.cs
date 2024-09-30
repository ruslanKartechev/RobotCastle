using DG.Tweening;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
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
        public IButtonInput BtnSpawn => _btnSpawn;
        
        
        [SerializeField] private TroopsCountUI _troopsCount;
        [SerializeField] private MoneyUI _money;
        [SerializeField] private MyButton _btnStart;
        [SerializeField] private MyButton _btnSpawn;
        [SerializeField] private TroopSizePurchaseUI _troopSizePurchaseUI;
        [SerializeField] private InvasionLevelsUI _levelUI;
        [SerializeField] private Vector2 _blockYPositions;
        [SerializeField] private RectTransform _mainRect;
        [SerializeField] private MyButton _btnStartTop;
        [SerializeField] private FadeInOutAnimator _startTopBtnAnimator;
        [SerializeField] private FadePopAnimator _winAnimator;
        [SerializeField] private FadePopAnimator _lostAnimator;
        [SerializeField] private FadePopAnimator _startedAnimator;

        private Battle _battle;
        private InvasionLevelData _levelData;
        
        public void Init(Battle battle, InvasionLevelData levelData)
        {
            _battle = battle;
            _levelData = levelData;
            _levelUI.LevelName = levelData.viewName;
            _levelUI.SetRewardForWave(levelData.levels[battle.stageIndex].reward);
            _levelUI.SetLevel(battle.stageIndex, levelData.levels, false);
        }
        
        public void UpdateForNextWave()
        {
            _levelUI.SetRewardForWave(_levelData.levels[_battle.stageIndex].reward);
            _levelUI.SetLevel(_battle.stageIndex, _levelData.levels, true);
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
