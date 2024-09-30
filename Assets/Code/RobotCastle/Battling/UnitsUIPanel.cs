using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitsUIPanel : MonoBehaviour, IScreenUI
    {
        [SerializeField] private UnitUITracker _prefab;
        [SerializeField] private UnitUITracker _prefabEnemy;

        public void AssignEnemyUI(HeroView view)
        {
            var instance = Instantiate(_prefabEnemy, transform);
            instance.SetTarget(view.UITrackingPoint);
            view.heroUI = instance.ui;
        }        
        
        public void AssignHeroUI(HeroView view)
        {
            var instance = Instantiate(_prefab, transform);
            instance.SetTarget(view.UITrackingPoint);
            view.heroUI = instance.ui;
        }
    }
}