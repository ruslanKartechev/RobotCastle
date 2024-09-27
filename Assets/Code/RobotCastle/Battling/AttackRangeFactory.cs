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
                    return new AttackRangeRectangle(4, 7);
                case "shelda":
                    return new AttackRangeSingle();
                case "joi":
                    return new AttackRangeSingle();
                case "leronhard":
                    return new AttackRangeSingle();
                case "mel":
                    return new AttackRangeSingle();
                case "evan":
                    return new AttackRangeRectangle(1, 1);
                case "priya":
                    return new AttackRangeRectangle(2, 4);
                case "hansi":
                    return new AttackRangeRectangle(4, 6);
                case "asiaq":
                    return new AttackRangeRectangle(3, 3);
                case "maiu":
                    return new AttackRangeRectangle(3, 3);
                case "alberon":
                    return new AttackRangeRectangle(2, 2);
                
                case "mushroom_1":
                    return new AttackRangeRectangle(1, 1);
                case "mushroom_2":
                    return new AttackRangeRectangle(1, 1);
                case "mushroom_3":
                    return new AttackRangeRectangle(1, 1);
                case "flower":
                    return new AttackRangeRectangle(2, 2);
                case "shade":
                    return new AttackRangeSingle();
                case "bat":
                    return new AttackRangeRectangle(1, 1);
                case "bumble":
                    return new AttackRangeRectangle(1, 1);
            }
            CLog.Log($"[AttackRangeFactory] No preset attack range for: {id}. Returning basic 1x1 \"PlusShape\" ");
            return new AttackRangeSingle();
        }
    }
}