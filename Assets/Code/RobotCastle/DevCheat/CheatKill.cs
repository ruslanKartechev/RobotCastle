using System.Collections.Generic;
using RobotCastle.Battling;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    public class CheatKill : MonoBehaviour
    {
        public GameObject hero;
        public List<GameObject> heroes;

        public void Kill()
        {
            var h = hero.GetComponent<IHeroController>();
            if (h != null)
            {
                h.Components.killProcessor.Kill();
            }
        }

        public void KillList()
        {
            foreach (var go in heroes)
            {
                var h = go.GetComponent<IHeroController>();
                if (h != null)
                {
                    h.Components.killProcessor.Kill();
                }
            }
        }
    }
}