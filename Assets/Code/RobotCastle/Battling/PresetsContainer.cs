using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/PresetsContainer", fileName = "PresetsContainer", order = 0)]
    public class PresetsContainer : ScriptableObject
    {
        [SerializeField] private List<string> _presets;

        public List<string> Presets => _presets;
    }
}