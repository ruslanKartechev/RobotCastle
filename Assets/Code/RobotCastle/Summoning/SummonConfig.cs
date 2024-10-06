using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.Summoning
{
    [System.Serializable]
    public class SummonConfig
    {
        public const int HeroMedalsIfAllHeroesOwned = 100;
        
        public const string Id_OwnedHero = "owned_hero";
        public const string Id_NewHero = "new_hero";
        public const string Id_Gold = "gold";
        public const string Id_Book = "book";
        public const string Id_KingMedal = "king_medal";
        public const string Id_HeroMedal = "hero_medal";
        public const string Id_UpgradeCube = "upgrade_cube";
        public const string Id_AnyInventoryItem = "inventory_item";

        public int outputItemsCount;
        public List<Option> options;

        [System.Serializable]
        public class Option
        {
            public string id;
            public float chance;
            public int amount;
            public int amountMax;
        }
    }


    public class SummonOutput
    {
        /// <summary>
        /// type - type of the item
        /// id - particular Id, i.e. hero id
        /// level - amount of item 
        /// </summary>
        public CoreItemData data;

        public SummonOutput(string type, string id, int amount)
        {
            data = new CoreItemData(amount, id, type);
        }
    }

    
}