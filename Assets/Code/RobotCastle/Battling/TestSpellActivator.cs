using UnityEngine;

namespace RobotCastle.Battling
{
    public class TestSpellActivator : IFullManaListener
    {
        public void OnFullMana(GameObject heroGo)
        {
            var hero = heroGo.GetComponent<IHeroController>();
            
        }
    }
}