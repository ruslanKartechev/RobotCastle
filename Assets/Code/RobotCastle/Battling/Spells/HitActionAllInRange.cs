using SleepDev;

namespace RobotCastle.Battling
{
    public class HitActionAllInRange : IAttackHitAction
    {
        public CellsMask mask;
        public HeroComponents components;

        public void Hit(object target)
        {
            var temp = (IDamageReceiver)target;
            if (temp == null)
            {
                CLog.LogRed("Damage receiver cast is null");
                return;
            }
            var pos = temp.GetGameObject().transform.position;
            components.movement.Map.GetCellAtPosition(pos, out var cellPos, out var cell);
            var allCells = mask.GetCellsAround(cellPos, components.movement.Map);
            var allEnemies = HeroesManager.GetHeroesEnemies(components);
            var args = components.damageSource.CalculatePhysDamage();
            for (var i = allEnemies.Count-1; i >= 0 ; i--)
            {
                var hh = allEnemies[i];
                if (allCells.Contains(hh.Components.movement.CurrentCell))
                    components.damageSource.Damage(hh.Components.damageReceiver, args);
            }

            components.stats.ManaAdder.AddDefault();
        }
    }
}