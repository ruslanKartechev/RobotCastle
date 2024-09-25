using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Saving;

namespace RobotCastle.Battling
{
    public static class HeroesHelper
    {
        public static int GetHeroLevel(string id)
        {
            var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(id);
            return heroSave.level;
        }

        public static List<ModifierProvider> GetModifiers(List<string> ids)
        {
            if (ids.Count == 0)
                return new List<ModifierProvider>();
            var db = ServiceLocator.Get<ModifiersDataBase>();
            var list = new List<ModifierProvider>(3);
            foreach (var id in ids)
                list.Add(db.GetModifier(id));
            return list;
        }

        public static List<ModifierProvider> GetModifiersForHero(string id)
        {
            var spellInfo = ServiceLocator.Get<HeroesDatabase>().GetHeroSpellInfo(id);
            var db = ServiceLocator.Get<ModifiersDataBase>();
            var list = new List<ModifierProvider>(3);
            if(spellInfo.mainSpellId.Length > 0)
                list.Add(db.GetSpell(spellInfo.mainSpellId));
            return list;
        }
    }
}