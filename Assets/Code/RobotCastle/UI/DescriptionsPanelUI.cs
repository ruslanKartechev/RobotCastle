using System.Collections;
using System.Collections.Generic;
using RobotCastle.Core;
using UnityEngine;
using UnityEngine.UI;
using SleepDev;

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
        private Coroutine _waitingInput;

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
            _clickTime = Time.time;
            Hide();
        }

        private void OnUpMain(Vector3 screenPos)
        {
            if (Time.time - _clickTime < _clickMaxDelay)
            {
                if (TryRaycastWorld(screenPos))
                {
                    return;
                }
                if (TryRaycastUI())
                {
                    return;
                }
            }
        }
        
        private void Hide()
        {
            if (_currentDescription != null)
            {
                _currentDescription.Hide();
                _currentDescription = null;
                _provider = null;
            }  
        }
        
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