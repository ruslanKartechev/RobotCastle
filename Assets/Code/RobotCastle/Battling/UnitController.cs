using Bomber;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private UnitView _unitView;
        private IUnitBehaviour _currentId;
        private bool _didInit;
        
        public void InitUnit(bool force = false)
        {
            if (_didInit && !force)
                return;
            _didInit = true;
            _unitView.agent.InitAgent(ServiceLocator.Get<IMap>());
        }
        
    }
}