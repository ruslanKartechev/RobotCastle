using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class PerHeroDamageStatsUI : MonoBehaviour, IPoolItem
    {
        [SerializeField] private Image _physDamageFill;
        [SerializeField] private Image _magDamageFill;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _damageText;
        private BattleDamageStatsCollector.PerHeroData _data;
        
        
        public void InitData(BattleDamageStatsCollector.PerHeroData data)
        {
            _data = data;
            var db = ServiceLocator.Get<HeroesDatabase>();
            var info = db.GetHeroViewInfo(data.heroId);
            _nameText.text = info.name;
            _icon.sprite = ViewDataBase.GetSprite(info.iconId);
        }
        
            
        public void UpdateData(BattleDamageStatsUI.Mode mode)
        {
            switch (mode)
            {
                case BattleDamageStatsUI.Mode.Dealt:
                {
                    var total = _data.damageDealtMag + _data.damageDealtPhys;
                    _damageText.text = total.ToString();
                       if (total <= 0)
                       {
                           _physDamageFill.fillAmount = 0;
                           _magDamageFill.fillAmount = 0;
                       }
                       else
                       {
                           _physDamageFill.fillAmount = (float)_data.damageDealtPhys / total;
                           _magDamageFill.fillAmount = (float)_data.damageDealtMag / total;
                       }
                }
                    break;
                case BattleDamageStatsUI.Mode.Received:
                {
                    var total = _data.damageReceivedPhys + _data.damageReceivedMag;
                    _damageText.text = total.ToString();
                    if (total <= 0)
                    {
                        _physDamageFill.fillAmount = 0;
                        _magDamageFill.fillAmount = 0;
                    }
                    else
                    {
                        _physDamageFill.fillAmount = (float)_data.damageReceivedPhys / total;
                        _magDamageFill.fillAmount = (float)_data.damageReceivedMag / total;
                    }
                }
                    break;
            }
        }

        public GameObject GetGameObject() => gameObject;

        public string PoolId { get; set; }
        
        public void PoolHide() => gameObject.SetActive(false);

        public void PoolShow() => gameObject.SetActive(true);
    }
}