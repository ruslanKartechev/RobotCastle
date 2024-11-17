using System;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class BarracksHeroView : MonoBehaviour
    {
        public event Action OnHeroShown;
        
        public string HeroId { get; private set; }
        
        [SerializeField] private Transform _heroPoint;
        private BarracksHeroPanel _descriptionPanel;
        
        public void ShowHero(string heroId)
        {
            HeroId = heroId;
            var pool = ServiceLocator.Get<BarracksHeroesPool>();
            var hero = pool.GetHero(heroId);
            var tr = hero.Transform;
            tr.DOKill();
            tr.gameObject.SetActive(true);
            tr.SetPositionAndRotation(_heroPoint.position, _heroPoint.rotation);
            if (_descriptionPanel == null)
            {
                _descriptionPanel = ServiceLocator.Get<IUIManager>().Show<BarracksHeroPanel>(UIConstants.UIBarracksHeroDescription, () => {});
            }
            _descriptionPanel.Show(heroId);
            OnHeroShown?.Invoke();
        }

        public void Close()
        {
            if (_descriptionPanel != null)
                _descriptionPanel.Close();
        }
    }
}