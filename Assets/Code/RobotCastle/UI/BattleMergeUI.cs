using RobotCastle.Battling;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BattleMergeUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private TroopsCountUI _troopsCount;
        [SerializeField] private MoneyUI _money;
        [SerializeField] private MyButton _btnStart;
        [SerializeField] private MyButton _btnSpawn;
        [SerializeField] private TroopSizePurchaseUI _troopSizePurchaseUI;
        [SerializeField] private LevelUI _levelUI;
        
        public LevelUI Level => _levelUI;
        public TroopSizePurchaseUI TroopSizePurchaseUI => _troopSizePurchaseUI;
        public IButtonInput BtnStart => _btnStart;
        
        public IButtonInput BtnSpawn => _btnSpawn;
        

        private void OnEnable()
        {
            ServiceLocator.Bind<ITroopsCountView>(_troopsCount);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<ITroopsCountView>();
        }
    }
}