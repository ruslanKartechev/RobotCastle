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
        [SerializeField] private MyButton _btnExpand;
        [SerializeField] private MyButton _btnSpawn;

        public IButtonInput BtnStart => _btnStart;
        
        public IButtonInput BtnExpand => _btnExpand;
        
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