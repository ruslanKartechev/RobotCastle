﻿using RobotCastle.Core;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class BarracksHeroView : MonoBehaviour
    {
        [SerializeField] private Transform _heroPoint;
        private BarracksHeroDescriptionPanel _descriptionPanel;
        
        public void ShowHero(string heroId)
        {
            var pool = ServiceLocator.Get<BarracksHeroesPool>();
            var hero = pool.GetHero(heroId);
            var tr = hero.Transform;
            tr.gameObject.SetActive(true);
            tr.SetPositionAndRotation(_heroPoint.position, _heroPoint.rotation);
            if (_descriptionPanel == null)
            {
                _descriptionPanel = ServiceLocator.Get<IUIManager>().Show<BarracksHeroDescriptionPanel>(UIConstants.UIBarracksHeroDescription, () => {});
            }
            _descriptionPanel.Show(heroId);
        }

        public void Close()
        {
            if (_descriptionPanel != null)
                _descriptionPanel.Close();
        }
    }
}