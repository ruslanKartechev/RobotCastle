using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class ShopTabsSwitcher : MonoBehaviour
    {
        [SerializeField] private List<TabData> _tabData;
        private TabData _current;

        public void InitFirstTab(int firstTab = 1)
        {
            for (var i = 0; i < _tabData.Count; i++)
            {
                var ind = i;
                _tabData[i].tabBtn.Id = ind;
                _tabData[i].tabBtn.AddMainCallback(() =>
                {
                    SetOn(_tabData[ind].tabBtn.Id);
                });
            }
            if (firstTab < 0 || firstTab >= _tabData.Count)
                firstTab = 0;
            SetOn(firstTab);
        }

        public void SetOn(int index)
        {
            if (_current != null)
            {
                _current.tab.Hide();
                _current.tabBtn.SetNotPicked();
            }
            _current = _tabData[index];
            _current.tab.Show();
            _current.tabBtn.SetPicked();
        }
        

        [System.Serializable]
        public class TabData
        {
            public ShopTab tab;
            public ShopTabBtn tabBtn;
        }
    }
}