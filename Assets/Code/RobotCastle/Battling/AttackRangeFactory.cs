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
                    return new AttackRangeSquare(6, 6);
                    break;
            }
            CLog.Log($"[AttackRangeFactory] No preset attack range for: {id}. Returning basic 1x1 \"PlusShape\" ");
            return new AttackRangeSingle();
        }
    }
}