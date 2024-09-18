using System.Collections.Generic;
using System.Threading;
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
        [SerializeField] private List<HeroView> _units;
        private List<HeroController> _initedUnits = new();
        

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
            _units = new List<HeroView>(FindObjectsOfType<HeroView>());
            foreach (var unit in _units)
            {
                var uc = unit.gameObject.GetComponent<HeroController>();
                if (uc != null)
                {
                    if (_initedUnits.Contains(uc) == false)
                    {
                        uc.InitComponents("aramis", 1, 1);
                        uc.PrepareForBattle();
                        _initedUnits.Add(uc);
                    }
                    uc.UpdateMap();
                }
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
            
            var uc = unit.gameObject.GetComponent<HeroController>();
            if (_initedUnits.Contains(uc) == false)
            {
                uc.InitComponents("aramis", 0, 0);
                uc.PrepareForBattle();
                _initedUnits.Add(uc);
            }
            
            uc.UpdateMap();
            
            var mover = unit.gameObject.GetComponent<HeroMovementManager>();
            var token = new CancellationTokenSource();
            mover.MoveToCell(pos, token.Token);
        }
#endif
    }
}