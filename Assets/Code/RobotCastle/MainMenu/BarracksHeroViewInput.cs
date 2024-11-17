using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public interface IItemValidator
    {
        bool CheckIfValid(IItemView itemView);
    }
    
    public class BarracksHeroViewInput : MonoBehaviour
    {
        
        public IItemValidator Validator { get; set; }
        
        public void SetActive(bool active)
        {
            CLog.Log($"[{nameof(BarracksHeroViewInput)}] Set Active: {active}, prev: {_isActive}");
            if (active == _isActive) return;
            if (active)
                ServiceLocator.Get<GameInput>().OnShortClick += OnShortClick;
            else
                ServiceLocator.Get<GameInput>().OnShortClick -= OnShortClick;
            _isActive = active;
        }

        [SerializeField] private LayerMask _layerMask;
        private bool _isActive;
        
        private void OnShortClick(Vector3 screenPos)
        {
            var ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit, 100, _layerMask))
            {
                if (hit.collider.gameObject.TryGetComponent<IItemView>(out var itemView))
                {
                    TryOpen(itemView); 
                }
                else if (hit.collider.gameObject.TryGetComponent<ICellView>(out var cellView))
                {
                    itemView = cellView.itemView;
                    if(itemView != null)
                        TryOpen(itemView);
                }
            }

            void TryOpen(IItemView itemView)
            {
                if (Validator != null
                    && !(Validator.CheckIfValid(itemView)))
                {
                    return;
                }
                if (itemView.itemData.core.type == MergeConstants.TypeHeroes)
                {
                    var id = itemView.itemData.core.id;
                    ServiceLocator.Get<TabsSwitcher>().SetHeroView();
                    ServiceLocator.Get<BarracksHeroView>().ShowHero(id);
                }
            }
        }
    }
}