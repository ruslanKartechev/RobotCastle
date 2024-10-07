using RobotCastle.Data;

namespace RobotCastle.Summoning
{
    [System.Serializable]
    public class SummoningDataBase
    {
        public ScrollConfig tier_1;
        public ScrollConfig tier_2;
        public ScrollConfig tier_3;
        public ScrollConfig tier_4;

        public ScrollConfig GetConfig(string id)
        {
            switch (id)
            {
                case ItemsIds.Scroll1:
                    return tier_1;
                case ItemsIds.Scroll2:
                    return tier_2;
                case ItemsIds.Scroll3:
                    return tier_3;
                case ItemsIds.Scroll4:
                    return tier_4;
            }
            return null;
        }
    }
}