using SleepDev;

namespace RobotCastle.Battling
{
    public class AttackRangeFactory
    {
        public static IAttackRange GetAttackRangeById(string id)
        {
            switch (id)
            {
                case "aramis":
                    return new AttackRangeSquare(4, 7);
                case "shelda":
                    return new AttackRangeSingle();
                case "joi":
                    return new AttackRangeSingle();
                case "leronhard":
                    return new AttackRangeSingle();
                case "mel":
                    return new AttackRangeSingle();
                case "evan":
                    return new AttackRangeSquare(1, 1);
                case "priya":
                    return new AttackRangeSquare(2, 4);
                case "hansi":
                    return new AttackRangeSquare(4, 6);
                case "asiaq":
                    return new AttackRangeSquare(3, 3);
                case "maiu":
                    return new AttackRangeSquare(3, 3);
    
            }
            CLog.Log($"[AttackRangeFactory] No preset attack range for: {id}. Returning basic 1x1 \"PlusShape\" ");
            return new AttackRangeSingle();
        }
    }
}