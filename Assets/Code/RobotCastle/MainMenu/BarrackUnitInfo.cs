using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.MainMenu
{
    public class BarrackUnitInfo : MonoBehaviour, IPoolItem
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _xpText;
        [SerializeField] private Image _xpFill;
        [SerializeField] private TextMeshProUGUI _notOwnedText;
        [SerializeField] private UIWorldPositionTracker _tracker;

        public UIWorldPositionTracker tracker => _tracker;
        

        public void ShowInfo(string heroId)
        {
            var saves = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
            var data = saves.GetSave(heroId);
            if (data.isUnlocked == false)
            {
                ShowNotOwned(heroId);
                return;
            }
            var db = ServiceLocator.Get<HeroesDatabase>();
            var info = db.GetHeroInfo(heroId);
            ShowInfo(info.viewInfo.name, data.level, data.xp, data.xpForNext);
        }
        
        public void ShowNotOwned(string name)
        {
            _nameText.text = name;
            _levelText.text = 1.ToString();
            _xpText.text = $"0/5";
            _xpFill.fillAmount = 0f;
            
            _xpFill.gameObject.SetActive(false);
            _xpText.gameObject.SetActive(false);
            _levelText.gameObject.SetActive(false);
            _notOwnedText.gameObject.SetActive(true);
        }

        public void ShowInfo(string name, int level, int xp, int xpMax)
        {
            _notOwnedText.gameObject.SetActive(false);
            _xpFill.gameObject.SetActive(true);
            _xpText.gameObject.SetActive(true);
            _levelText.gameObject.SetActive(true);
            
            _nameText.text = name;
            _levelText.text = (level + 1).ToString();
            _xpText.text = $"{xp}/{xpMax}";
            _xpFill.fillAmount = (float)xp / xpMax;
        }

        public GameObject GetGameObject() => gameObject;

        public string PoolId { get; set; }
        
        public void PoolHide() => gameObject.SetActive(false);

        public void PoolShow() => gameObject.SetActive(true);
    }
}