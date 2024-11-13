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
                    return new AttackRangeRectangle(1,1);
                case "alberon":
                    return new AttackRangeRectangle(2, 2);
                case "zupitere":
                    return new AttackRangeRectangle(3, 3);
                case "rie":
                    return new AttackRangeRectangle(3, 3);
                case "doll":
                    return new AttackRangeRectangle(1, 1);
                                
                case "saras":
                    return new AttackRangeRectangle(4, 4);
                case "bombie":
                    return new AttackRangeRectangle(3, 3);
                case "behemus":
                    return new AttackRangeSingle();
                case "draco":
                    return new AttackRangeRhombus(2);
                
                case "mushroom_1":
                    return new AttackRangeSingle();
                case "mushroom_2":
                    return new AttackRangeSingle();
                case "mushroom_3":
                    return new AttackRangeSingle();
                case "flower":
                    return new AttackRangeRhombus(2);
                case "shade":
                    return new AttackRangeRectangle(2,2);
                case "bat":
                    return new AttackRangeRectangle(1, 1);
                case "bumble":
                    return new AttackRangeRectangle(1, 1);
                case "bloom":
                    return new AttackRangeRectangle(2, 3);
                case "bat_lord":
                    return new AttackRangeRectangle(2, 3);
                case "devil_hell":
                    return new AttackRangeRectangle(2, 3);
                case "devil_hades":
                    return new AttackRangeRectangle(3, 4);
                case "vampire_bat":
                    return new AttackRangeRectangle(1, 1);
                case "spider_king":
                    return new AttackRangeRectangle(1, 1);
                case "spider_toxin":
                    return new AttackRangeRectangle(4, 4);
                case "dog":
                    return new AttackRangeRectangle(1, 1);
                case "cat":
                    return new AttackRangeRectangle(1, 1);
                case "cyclop_small":
                    return new AttackRangeRectangle(3, 3);
                case "cyplop_big":
                    return new AttackRangeRectangle(2, 3);
                case "scorpion_norm":
                    return new AttackRangeRectangle(1, 1);
                case "scorpion_king":
                    return new AttackRangeRectangle(1, 1);
                case "cactus":
                    return new AttackRangeRectangle(1, 1);
                case "gigacactus":
                    return new AttackRangeRectangle(6, 14);
                case "scorpling":
                    return new AttackRangeSingle();
                case "queen_worm":
                    return new AttackRangeRectangle(1,1);
                case "sprout":
                    return new AttackRangeRectangle(3,6);
                case "mole_rat":
                    return new AttackRangeRectangle(2,5);
                case "burrow":
                    return new AttackRangeRhombus(2);
                case "skelet_giant":
                    return new AttackRangeRectangle(2,2);
                
            }
            CLog.Log($"[AttackRangeFactory] No preset attack range for: {id}. Returning basic 1x1 \"PlusShape\" ");
            return new AttackRangeSingle();
        }
    }
}