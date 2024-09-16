using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitMoveBehaviour : MonoBehaviour, IUnitBehaviour
    {
        private Action<IUnitBehaviour> _callback;
        
        public string BehaviourID => "unit move";
        
        public void Activate(GameObject target, Action<IUnitBehaviour> endCallback)
        {
            var hero = target.GetComponent<HeroController>();
            Activate(hero, endCallback);
        }

        public void Activate(HeroController hero, Action<IUnitBehaviour> endCallback)
        {
            _callback = endCallback;
            
        }
        
        
    }
}