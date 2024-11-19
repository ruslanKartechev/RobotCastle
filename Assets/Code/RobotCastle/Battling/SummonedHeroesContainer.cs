using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public class SummonedHeroesContainer
    {
        public List<IHeroController> heroes = new (5);

        public void Add(IHeroController h)
        {
            heroes.Add(h);
        }

        public void Remove(IHeroController h)
        {
            heroes.Remove(h);
        }

        public void DestroyAll()
        {
            foreach (var h in heroes)
            {
                if (h == null)
                    continue;
                h.StopCurrentBehaviour();
                UnityEngine.Object.Destroy(h.Components.gameObject);
            }
            heroes.Clear();
        }
        
    }
}