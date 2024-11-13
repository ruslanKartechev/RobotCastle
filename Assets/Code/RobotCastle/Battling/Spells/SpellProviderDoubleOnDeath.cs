using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Double On Death", fileName = "double_on_death", order = 0)]
    public class SpellProviderDoubleOnDeath : SpellProvider
    {
        [SerializeField] private List<SpawnArgs> _spawnArgs;


        public override void AddTo(GameObject target)
        {
            if (target.TryGetComponent<HeroComponents>(out var comp))
                AddToHero(comp);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            var spell = new SpellDoubleWhenKilled(_spawnArgs, components);
        }

        public override float manaMax => 100;
        public override float manaStart => 0;

        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            return str;
        }
    }
}