using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.MainMenu;
using UnityEngine;

namespace RobotCastle.UI
{
    public class TabsHighlighterUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _highlights;
        private MenuTabType _current;
        
        public void HighlightTab(MenuTabType type)
        {
            OffCurrent();
            switch (type)
            {
                case MenuTabType.Barracks:
                    _highlights[0].SetActive(true);
                    break;
                case MenuTabType.Gate:
                    _highlights[1].SetActive(true);
                    break;
                case MenuTabType.Shop:
                    _highlights[2].SetActive(true);                    
                    break;
            }
            _current = type;
        }

        public void OffCurrent()
        {
            switch (_current)
            {
                case MenuTabType.Barracks:
                    _highlights[0].SetActive(false);
                    break;
                case MenuTabType.Gate:
                    _highlights[1].SetActive(false);
                    break;
                case MenuTabType.Shop:
                    _highlights[2].SetActive(false);                    
                    break;
            }
        }

        private void OnEnable()
        {
            ServiceLocator.Bind<TabsHighlighterUI>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<TabsHighlighterUI>();
        }
    }
}