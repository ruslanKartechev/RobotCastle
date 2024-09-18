using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroRangeCoverCheck
    {
        private HeroController _hero;
        private Vector2Int _center;
        private List<Vector2Int> _coveredCells;

        public HeroRangeCoverCheck(HeroController hero)
        {
            _hero = hero;
            _coveredCells = new(20);
        }
        
        public void Update(Vector3 worldPos, bool force)
        {
            var centerCell = _hero.HeroView.agent.Map.GetCellPositionFromWorld(worldPos);
            if (force)
            {
                _center = centerCell;
                RecalculateCovered();
                return;
            }

            if (_center != centerCell)
            {
                _center = centerCell;
                RecalculateCovered();
            }            
        }
        
        public void Update(Vector2Int centerCell, bool force)
        {
            if (force)
            {
                _center = centerCell;
                RecalculateCovered();
                return;
            }

            if (_center != centerCell)
            {
                _center = centerCell;
                RecalculateCovered();
            }            
        }

        public void RecalculateCovered()
        {
            var map = _hero.HeroView.agent.Map;
            var myPos = map.GetCellPositionFromWorld(_hero.transform.position);
            var cellsMask = _hero.HeroView.Stats.Range.GetCellsMask();
            _coveredCells.Clear();
            foreach (var val in cellsMask)
                _coveredCells.Add(myPos + val);
        }

        public bool IsHeroWithinRange(HeroController otherHero)
        {
            var cellPos = _hero.HeroView.agent.Map.GetCellPositionFromWorld(otherHero.HeroView.transform.position);
            if (_coveredCells.Contains(cellPos))
                return true;
            return false;
        }
    }
}