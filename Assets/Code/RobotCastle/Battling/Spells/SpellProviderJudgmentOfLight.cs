using UnityEngine;

namespace RobotCastle.Battling
{
    /// <summary>
    /// Judgment of Light - Smashes his hammer down with all his might,
    /// dealing 50/60/70/80+SP damage to all enemies in 1/3x3/2/5x5 block(s) nearby.
    /// </summary>
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider JudgmentOfLight", fileName = "judgement_of_light", order = 0)]
    public class SpellProviderJudgmentOfLight : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigJudgementOfLight _config;

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroView view)
        {
        }
    }
}