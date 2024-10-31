using System.Collections.Generic;
using RobotCastle.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Relics
{
    public class RelicItemUI : MonoBehaviour, IPoolItem
    {
  
        public RelicData relicData { get; set; }
        
        public RelicSave relicSave { get; set; }
        
        public void SetEmpty()
        {
            relicData = null;
            relicSave = null;
            _icon.sprite = null;
            _icon.color = Color.black;
            _stats.Off();
        }

        public Sprite GetIcon() => _icon.sprite;
        
        public void SetDataAndIcon(RelicData data, RelicSave save, Sprite icon)
        {
            _icon.sprite = icon;
            relicData = data;
            relicSave = save;
            _icon.color = Color.white;
            SetEquipped(relicSave.isEquipped);
            _stats.On();
            var mod = RelicsManager.GetStatModFromRelic(relicData) ;
            if (mod != null)
                _stats.ShowStatMod(mod);
        }

        public void SetEquipped(bool equipped)
        {
            if (equipped)
            {
                foreach (var go in _equippedObjects)
                    go.SetActive(true);
            }
            else
            {
                foreach (var go in _equippedObjects)
                    go.SetActive(false);
            }
        }
        
        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }

        public GameObject GetGameObject() => gameObject;

        public string PoolId { get; set; }
        
        public void PoolHide() => gameObject.SetActive(false);

        public void PoolShow() => gameObject.SetActive(true);

        public void OnPicked()
        {
            transform.localScale = Vector3.one * .85f;
        }

        public void OnPut()
        {
            transform.localScale = Vector3.one;
        }
        
        
        [SerializeField] private Image _icon;
        [SerializeField] private List<GameObject> _equippedObjects;
        [SerializeField] private RelicStatsUI _stats;
    }
}