using UnityEngine;

namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class DifficultyTierConfig 
    {
        public float enemyForcesPercent => _enemyForcesPercent;

        public int enemyTier => _enemyTier;

        public int castleDurabilityPenalty => _castleDurabilityPenalty;

        public int challenngePhaseEliteEnemies => _challenngePhaseEliteEnemies;

        public int additionalEliteEnemies => _additionalEliteEnemies;

        public string textDescription
        {
            get => _textDescription;
            set => _textDescription = value;
        }

        [SerializeField] private float _enemyForcesPercent;
        [SerializeField] private int _enemyTier;
        [SerializeField] private int _challenngePhaseEliteEnemies;
        [SerializeField] private int _castleDurabilityPenalty;
        [SerializeField] private int _additionalEliteEnemies;
        [Space(10)]
        [SerializeField] private string _textDescription;
    }
}