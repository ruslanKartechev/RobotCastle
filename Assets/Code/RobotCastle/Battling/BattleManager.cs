using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(10)]
    public class BattleManager : MonoBehaviour
    {
        private BattleData _battleData;
        
        public BattleData BattleData => _battleData;

        public static HeroController GetBestTargetForAttack(GameObject hero, BattleTeam myTeam)
        {
            
            
            return null;
        }
        
        
        
        
        
        
        
        
        private void Awake()
        {
            _battleData = BattleData.GetDefault();
            ServiceLocator.Bind<BattleData>(_battleData);
            CLog.Log($"[{nameof(BattleManager)}] Created new battle data");
        }
        
        private void OnEnable()
        {
            ServiceLocator.Bind<BattleManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<BattleManager>();
        }
        
        
    }
}