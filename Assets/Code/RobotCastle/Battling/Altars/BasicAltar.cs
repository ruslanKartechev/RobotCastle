using SleepDev;

namespace RobotCastle.Battling.Altars
{
    public class BasicAltar : Altar
    {
        protected AltarMP GetModifier(int level)
        {
            return _modifiers[level];
        }

        public override void AddPoint()
        {
            if (_points == MaxPoints) return;
            SetPoints(_points + 1);
        }

        public override void RemovePoints()
        {
            if (_points == 0) return;
            SetPoints(_points - 1);
        }
        
        public override void SetPoints(int points)
        {
            var prevPoints = _points;
            _points = points;
            if (points == 0)
            {
                foreach (var mod in _modifiers)
                    mod.SetTier(0);
            }
            else
            {
                const int max = 5;
                var msg = $"[SetPoints] Altar: {ViewName}";
                for (var modId = 0; modId < _modifiers.Count; modId++)
                {
                    var pointsLeft = points - modId * max;
                    if (modId > 0)
                        pointsLeft += 1;
                    if(pointsLeft < 0)
                        pointsLeft = 0;
                    var mod = _modifiers[modId];
                    msg += $"\nMod_{modId + 1}, tier_{pointsLeft}";
                    mod.SetTier(pointsLeft);
                }
                CLog.Log(msg);
            }
            RaisePointsUpdated(prevPoints, points);
        }
    }
    
}