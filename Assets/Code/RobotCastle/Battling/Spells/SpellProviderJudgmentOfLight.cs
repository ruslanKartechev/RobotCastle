using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider JudgmentOfLight", fileName = "judgement_of_light", order = 0)]
    public class SpellProviderJudgmentOfLight : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigJudgementOfLight _config;

        public override void AddTo(GameObject target)
        {
        }
        
        public override void AddToHero(HeroView view)
        {
            view.stats.FullManaListener = new SpellJudgementOfLight(view, _config);
          
        }
    }
}