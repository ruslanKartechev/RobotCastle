using DG.Tweening;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class BattleCamera : MonoBehaviour
    {
        [SerializeField] private float _moveTime;
        [SerializeField] private Transform _mergePoint;
        [SerializeField] private Transform _battlePoint;

        public void MoveToBattlePoint()
        {
            transform.DOKill();
            transform.DOMove(_battlePoint.position, _moveTime).SetEase(Ease.Linear);
        }

        public void MoveToMergePoint()
        {
            transform.DOKill();
            transform.DOMove(_mergePoint.position, _moveTime).SetEase(Ease.Linear);
        }
        
    }
}