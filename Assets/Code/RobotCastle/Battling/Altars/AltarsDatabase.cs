using System.Collections.Generic;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    
    [CreateAssetMenu(menuName = "SO/Altars/Altars Database", fileName = "Altars Database", order = -1)]
    public class AltarsDatabase : ScriptableObject
    {
        [SerializeField] private List<float> _nextPointCost;
        [SerializeField] private List<Altar> _altars;

        public Altar GetAltar(int index) => _altars[index];

        public int GetIndexOf(Altar altar) => _altars.IndexOf(altar);

        
        /// <param name="playerLevel">(NOT index) Level of castle 1,2,3,4 etc.</param>
        public float GetNextPointCost(int playerLevel)
        {
            if(playerLevel < _nextPointCost.Count)
                return _nextPointCost[playerLevel-1];
            return 0;
        }

     
        /// <param name="currentPointsTotal">Total amount of altar points</param>
        /// <param name="playerLevel">(NOT index) Level of castle 1,2,3,4 etc.</param>
        /// <returns></returns>
        public bool CanBuyMorePoints(int currentPointsTotal, int playerLevel)
        {
            if (playerLevel > currentPointsTotal)
                return true;
            return false;
        }

        public bool HasReachedMaxPoints(int totalPoint) => totalPoint >= 90;
        
        /// <summary>
        /// Calls SetupData()
        /// Apply all active altar modifiers
        /// </summary>
        public void ApplyAllActiveModifiers()
        {
            
        }
    }
}