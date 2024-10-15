using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.MainMenu;
using UnityEngine;

namespace RobotCastle.UI
{
    public class TabsHighlighterUI : MonoBehaviour
    {
        [SerializeField] private float _animationTime;
        [SerializeField] private Sprite _normalIcon;
        [SerializeField] private Sprite _highlightIcon;
        [Space(5)]
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _highlightColor;
        
        [Space(5)]
        [SerializeField] private List<TabBtn> _highlights;
        
        private MenuTabType _current;
        
        public void HighlightTab(MenuTabType type)
        {
            OffCurrent();
            TabBtn btn = null;
            switch (type)
            {
                case MenuTabType.Barracks:
                    btn = _highlights[0];
                    break;
                case MenuTabType.Gate:
                    btn = _highlights[1];
                    break;
                case MenuTabType.Shop:
                    btn = _highlights[2];
                    break;
            }
            _current = type;
            btn.Animate(true, _highlightColor, _highlightIcon, _animationTime);
        }

        public void OffCurrent()
        {
            TabBtn btn = null;
            switch (_current)
            {
                case MenuTabType.Barracks:
                    btn = _highlights[0];
                    break;
                case MenuTabType.Gate:
                    btn = _highlights[1];
                    break;
                case MenuTabType.Shop:
                    btn = _highlights[2];
                    break;
                default: return;
            }
            btn.Animate(false, _normalColor, _normalIcon, _animationTime);
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