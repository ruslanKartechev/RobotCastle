using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public abstract class SpellProvider : ModifierProvider
    {
        public override string GetDescription(GameObject target)
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return db.GetDescription(_id).parts[1];
        }

        public virtual int GetTier(GameObject hero)
        {
            var merge = hero.GetComponent<HeroStatsContainer>().MergeTier;
            return HeroesConfig.GetSpellTier(hero.GetComponent<HeroStatsContainer>().MergeTier);
        }

        public virtual string GetName()
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return db.GetDescription(_id).parts[0];
        }
        
    }
}