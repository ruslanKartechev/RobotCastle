using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class ModifiersContainer : MonoBehaviour
    {
        [SerializeField] private List<ModifierProvider> _modifiers;

        public List<ModifierProvider> Modifiers => _modifiers;

        public void ApplyToSelfGamObject()
        {
            
        }

    }
}