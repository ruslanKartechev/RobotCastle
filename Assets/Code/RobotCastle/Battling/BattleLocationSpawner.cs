using RobotCastle.InvasionMode;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class BattleLocationSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
         
        public void SpawnLocation(Chapter chapter)
        {
            var prefab = Resources.Load<GameObject>($"prefabs/locations/{chapter.location}");
            if (prefab == null)
            {
                CLog.LogError($"Cannot load location prefab: prefabs/locations/{chapter.location}");
                return;
            }

            var inst = Instantiate(prefab, _parent);
        }
        
    }
}