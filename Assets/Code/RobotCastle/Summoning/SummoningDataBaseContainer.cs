using UnityEngine;

namespace RobotCastle.Summoning
{
    [CreateAssetMenu(menuName = "SO/Summoning DataBase Container", fileName = "Summoning DataBase Container", order = 0)]
    public class SummoningDataBaseContainer : ScriptableObject
    {
        public SummoningDataBase dataBase;
    }
}