using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Shop
{
    [CreateAssetMenu(menuName = "SO/InApps Data", fileName = "InApps Data", order = -100)]
    public class InAppsSKUDataBase : ScriptableObject
    {
        [SerializeField] private List<Data> _data;
        [SerializeField] private List<string> _purchasedSkus;

        [System.Serializable]
        private class Data
        {
            public string id;
            public bool isConsumable;
            public string skuAndroid;
            public string skuIOS;

            public string GetSKU()
            {
#if UNITY_ANDROID
                return skuAndroid;
#elif UNITY_IOS
                return skuIOS;
#endif
                return skuAndroid;
            }
        }

        private Dictionary<string, string> _skuById;
        public List<string> nonConsumables { get; set; }
        public List<string> consumables { get; set; }
        public List<string> purchasedNonConsumablesSKUs => _purchasedSkus;


        public string GetSKUWithID(string id)
        {
            return _skuById[id];
        }
        
        public void Init()
        {
            consumables = new List<string>(10);
            nonConsumables = new List<string>(10);
            _skuById = new Dictionary<string, string>(_data.Count);
            foreach (var dd in _data)
            {
                if(dd.isConsumable)
                    consumables.Add(dd.GetSKU());
                else
                    nonConsumables.Add(dd.GetSKU());
                _skuById.Add(dd.id, dd.GetSKU());
            }
        }
        
    }
}