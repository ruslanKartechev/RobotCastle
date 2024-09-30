using System;
using System.Threading.Tasks;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class BattleCamera : MonoBehaviour
    {
        public int SlideBlockers
        {
            get => _slideBlockers;
            set
            {
                _slideBlockers = value;
                if(_slideBlockers < 0)
                    _slideBlockers = 0;
            } 
        }

        public float MoveTime
        {
            get => _moveTime;
            set => _moveTime = value;
        }
        
        [SerializeField] private float _moveTime;
        [SerializeField] private Transform _mergePoint;
        [SerializeField] private Transform _battlePoint;
        private int _slideBlockers;
    

        public void AllowPlayerInput(bool allowed)
        {
            if(allowed)
                ServiceLocator.Get<GameInput>().OnSlideMain += OnSlide;
            else
                ServiceLocator.Get<GameInput>().OnSlideMain -= OnSlide;
        }

        private void OnSlide(Vector3 vec)
        {
            if (SlideBlockers > 0)
                return;
            if (Mathf.Abs(vec.y) > Math.Abs(vec.x))
            {
                if (vec.y < 0)
                {
                    MoveToBattlePoint();
                    var battleUI = ServiceLocator.Get<IUIManager>().GetIfShown<BattleMergeUI>(UIConstants.UIBattleMerge);
                    if(battleUI)
                        battleUI.SetMainAreaLowerPos();
                }
                else if (vec.y > 0)
                {
                    MoveToMergePoint();
                    var battleUI = ServiceLocator.Get<IUIManager>().GetIfShown<BattleMergeUI>(UIConstants.UIBattleMerge);
                    if(battleUI)
                        battleUI.SetMainAreaUpPos();
                }
            }
        }

        private void MoveUp()
        {
            MoveToBattlePoint();
        }

        private void MoveDown()
        {
            MoveToMergePoint();
        }

        public void SetMergePoint()
        {
            transform.position = _mergePoint.position;
        }

        public void SetBattlePoint()
        {
            transform.position = _battlePoint.position;
        }
        
        public async Task MoveToBattlePoint()
        {
            transform.DOKill();
            transform.DOMove(_battlePoint.position, _moveTime).SetEase(Ease.Linear);
            await Task.Delay((int)(_moveTime * 1000));

        }

        public async Task MoveToMergePoint()
        {
            transform.DOKill();
            transform.DOMove(_mergePoint.position, _moveTime).SetEase(Ease.Linear);
            await Task.Delay((int)(_moveTime * 1000));
        }
        
    }
}