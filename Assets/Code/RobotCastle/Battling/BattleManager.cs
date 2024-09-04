using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(10)]
    public class BattleManager : MonoBehaviour
    {
        private BattleData _battleData;

        private void Start()
        {
            _battleData = BattleData.GetDefault();
            ServiceLocator.Bind<BattleData>(_battleData);
            CLog.LogWhite($"[{nameof(BattleManager)}] Created new battle data");
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