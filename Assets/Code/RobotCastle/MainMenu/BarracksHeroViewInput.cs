using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class BarracksHeroViewInput : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        private bool _isActive;
        
        public void SetActive(bool active)
        {
            if (active == _isActive) return;
            if (active)
            {
                ServiceLocator.Get<GameInput>().OnShortClick += OnShortClick;
            }
            else
            {
                ServiceLocator.Get<GameInput>().OnShortClick -= OnShortClick;
            }
            _isActive = active;
        }

        private void OnShortClick(Vector3 screenPos)
        {
            var ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit, 100, _layerMask))
            {
                if (hit.collider.gameObject.TryGetComponent<IItemView>(out var itemView))
                {
                    if (itemView.itemData.core.type == MergeConstants.TypeUnits)
                    {
                        var id = itemView.itemData.core.id;
                        ServiceLocator.Get<TabsSwitcher>().SetHeroView();
                        ServiceLocator.Get<BarracksHeroView>().ShowHero(id);
                    }
                }
            }
        }
    }
}