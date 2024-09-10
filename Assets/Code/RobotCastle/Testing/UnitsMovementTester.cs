using System;
using System.Collections.Generic;
using Bomber;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class UnitsMovementTester : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private MapBuilder _mapBuilder;
        [SerializeField] private Transform _transform;
        [SerializeField] private List<UnitView> _units;

        private void Awake()
        {
            BuildMap();
        }

        public void BuildMap()
        {
            _mapBuilder.InitRuntime();
            ServiceLocator.Bind<IMap>(_mapBuilder.Map);
        }
        
        public void GetUnits()
        {
            _units = new List<UnitView>(FindObjectsOfType<UnitView>());
            foreach (var unit in _units)
            {
                var uc = unit.gameObject.GetComponent<UnitController>();
                if(uc != null)
                    uc.InitUnit();
            }
        }

        public void MoveOneToPos()
        {
            if (_units.Count == 0)
            {
                CLog.Log("No units in the list");
                return;
            }
            var map = ServiceLocator.Get<IMap>();
            map.GetCellAtPosition(_transform.position, out var pos, out var cell);
            var unit = _units[0];
            
            var uc = unit.gameObject.GetComponent<UnitController>();
            uc.InitUnit();
            
            var mover = unit.gameObject.GetComponent<UnitMover>();
            mover.MoveToCell(pos.x, pos.y);
        }
#endif
    }
}