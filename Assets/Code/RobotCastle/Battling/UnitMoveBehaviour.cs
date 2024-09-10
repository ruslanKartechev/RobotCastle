using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitMoveBehaviour : MonoBehaviour, IUnitBehaviour
    {
        public string BehaviourID => "unit move";
        
        public void Activate(GameObject target, Action<IUnitBehaviour> endCallback)
        {
            
        }
    }
}