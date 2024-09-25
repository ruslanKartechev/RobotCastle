using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class ModifiersContainer : MonoBehaviour
    {
        [SerializeField] private List<string> _modifierIds;

        public List<string> ModifierIds => _modifierIds;

    }
}