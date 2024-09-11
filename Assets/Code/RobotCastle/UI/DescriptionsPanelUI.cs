using System.Collections.Generic;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class DescriptionsPanelUI : MonoBehaviour
    {
        [SerializeField] private float _clickMaxDelay = .1f;
        [SerializeField] private Transform _parent;
        [SerializeField] private GraphicRaycaster _raycaster;
        private Dictionary<string, DescriptionUI> _spawned = new (5);
        private DescriptionUI _currentDescription;
        private IItemDescriptionProvider _provider;
        private float _clickTime;
        private int _clicks;

        private void OnEnable()
        {
            var inp = ServiceLocator.Get<GameInput>();
            inp.OnDownIgnoreUI += OnDownMain;
            inp.OnUpIgnoreUI += OnUpMain;
        }

        private void OnDisable()
        {
            var inp = ServiceLocator.Get<GameInput>();
            inp.OnDownIgnoreUI -= OnDownMain;
            inp.OnUpIgnoreUI -= OnUpMain;
        }

        private void OnDownMain(Vector3 screenPos)
        {
            _clicks++;
            if ((Time.time - _clickTime > _clickMaxDelay))
            {
                _clickTime = Time.time;
                _clicks = 0;
                if (_currentDescription != null)
                {
                    _currentDescription.Hide();
                    _currentDescription = null;
                    _provider = null;
                }  
                return;
            }
            _clickTime = Time.time;
            // CLog.LogWhite($"Count: {_clicks}");
            if (_clicks >= 1)
            {
                _clicks = 0;
                // CLog.LogGreen($"Trying to raycast");
                if (TryRaycastWorld(screenPos))
                { }
            }
        }
        
        private void OnUpMain(Vector3 screenPos)
        { }

        private bool TryRaycastWorld(Vector3 pos)
        {
            var ray = Camera.main.ScreenPointToRay(pos);
            var hits = Physics.RaycastAll(ray);
            if (hits.Length == 0)
                return false;
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<IItemDescriptionProvider>(out var provider))
                {
                    _provider = provider;
                    _currentDescription = GetUIForType(provider.GetIdForUI());
                    _currentDescription.Show(provider.GetGameObject());
                    return true;
                }
            }
            return false;
        }

        private bool TryRaycastUI()
        {
            
            return false;
        }

        private DescriptionUI GetUIForType(string type)
        {
            if (_spawned.ContainsKey(type) && _spawned[type] != null)
            {
                return _spawned[type];
            }
            var prefab = Resources.Load<DescriptionUI>($"prefabs/ui/descriptions/{type}");
            var inst = Instantiate(prefab, _parent);
            _spawned.Add(type, inst);
            return inst;
        }
    }
}