using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    public abstract class SpellProvider : ModifierProvider
    {
        public abstract float manaMax { get; }
        public abstract float manaStart { get; }

        public override string GetDescription(GameObject target)
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return db.GetDescription(_id).parts[1];
        }

        public virtual ESpellTier GetTier(GameObject hero)
        {
            return HeroesHelper.GetSpellTier(hero.GetComponent<HeroStatsManager>().MergeTier);
        }

        public virtual string GetName()
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return db.GetDescription(_id).parts[0];
        }
        
    }
}