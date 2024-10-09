using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public partial class BattleLevel
    {
#if UNITY_EDITOR

        [ContextMenu("Force SmeltingOffer")]
        public void ForceShowSmeltingOffer()
        {
            AllowPlayerUIInput(false);
            ShowSmeltingOffer();
        }

        [ContextMenu("Force TradeOffer")]
        public void ForceShowTradeOffer()
        {
            AllowPlayerUIInput(false);
            ShowTradeOffer();
        }

        [ContextMenu("Force DevilOffer")]
        public void ForceShowDevilOffer()
        {
            AllowPlayerUIInput(false);
            ShowDevilsOffer();
        }

        [ContextMenu("Force Fail")]
        public void ForceFail()
        {
            if (_battleManager.battle.State == BattleState.Going)
            {
                CLog.Log($"Battle still going");
                return;
            }
            AllowPlayerUIInput(false);
            ShowFailedScreen();
        }

        [ContextMenu("Force Win")]
        public void ForceWin()
        {
            if (_battleManager.battle.State == BattleState.Going)
            {
                CLog.Log($"Battle still going");
                return;
            }

            AllowPlayerUIInput(false);
            GiveRewardAndShowUI();
        }
#endif

    }
}