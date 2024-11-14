using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroStateData : IMoveInfoProvider
    {
        public bool isMoving;
        public bool isAttacking;
        public bool isStunned;
        public bool isPulled;
        public bool isOutOfMap;
        public Vector2Int targetMoveCell;
        public Vector2Int currentCell;
        public AttackTargetData attackData { get; set; } = new ();

        public Vector2Int TargetCell
        {
            get => targetMoveCell;
            set => targetMoveCell = value;
        }

        public void Reset()
        {
            isMoving = isAttacking = isStunned = isPulled = isOutOfMap = false;
            targetMoveCell = new Vector2Int(-1, -1);
            attackData.Reset();
        }
        
        public void SetTargetCellToSelf() => targetMoveCell = currentCell;

        public void SetOutOfMap(bool outOfMap) => this.isOutOfMap = outOfMap;

        public void SetStunned(bool stunned) => this.isStunned = stunned;

        public void SetPulled(bool pulled) => this.isPulled = pulled;

        public string GetStr()
        {
            return $"Moving: {isMoving}. Attacking: {isAttacking}. TargetCell: {targetMoveCell}. Current cell: {currentCell}. Stunned: {isStunned}";
        }
    }
}