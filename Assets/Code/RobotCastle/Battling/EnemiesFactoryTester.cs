using UnityEngine;

namespace RobotCastle.Battling
{
    public class EnemiesFactoryTester : MonoBehaviour
    {
        [SerializeField] private string _testPackId;    
        [SerializeField] private EnemiesFactory _factory;

        [ContextMenu("SpawnTestPack")]
        public void SpawnTestPack()
        {
            _factory.SpawnPreset(_testPackId);
            
        }
    }
}