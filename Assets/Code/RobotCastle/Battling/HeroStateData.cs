using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroStateData : IMoveInfoProvider
    {
        public bool isMoving;
        public bool isAttacking;
        public Vector2Int targetMoveCell;
        public Vector2Int currentCell;
        public bool isStunned;
        public AttackTargetData attackData { get; set; } = new ();

        public Vector2Int TargetCell
        {
            get => targetMoveCell;
            set => targetMoveCell = value;
        }

        public void Reset()
        {
            isMoving = isAttacking = isStunned = false;
            targetMoveCell = new Vector2Int(-1, -1);
            attackData.Reset();
        }
        
        public void SetTargetCellToSelf()
        {
            targetMoveCell = currentCell;
        }

        public string GetStr()
        {
            return $"Moving: {isMoving}. Attacking: {isAttacking}. TargetCell: {targetMoveCell}. Current cell: {currentCell}. Stunned: {isStunned}";
        }
    }
}