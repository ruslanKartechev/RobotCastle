using System;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroIdleBehaviour : IHeroBehaviour
    {
        public string BehaviourID => "hero_idle"; 

        public void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback)
        {
            var view = hero.View;
            view.movement.Stop();
            view.attackManager.Stop();
            view.agent.SetCellMoveCheck(null);
            view.animator.SetBool(HeroesConfig.Anim_Attack, false);
            view.animator.SetBool(HeroesConfig.Anim_Move, false);
        }

        public void Stop()
        { }
    }
}