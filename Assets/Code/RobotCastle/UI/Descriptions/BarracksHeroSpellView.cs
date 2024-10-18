using System;
using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class BarracksHeroSpellView : MonoBehaviour, IItemDescriptionProvider
    {
        public RectTransform pivotRect => _pivotRect;
        
        public HeroInfo heroInfo => _heroInfo;
        
        public GameObject heroGo => _heroGo;


        public void Init(string heroId) => throw new NotImplementedException();

        public void Init(string heroId, HeroInfo heroInfo)
        {
            this._heroId = heroId;
            this._heroInfo = heroInfo;
            this._heroGo = heroGo;
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            _icon.sprite = viewDb.GetSpellIcon(heroInfo.spellInfo.mainSpellId);
        }
        
        public string GetIdForUI() => _uiId;

        public GameObject GetGameObject() => gameObject;

        public Vector3 WorldPosition { get; }
        
                
        [SerializeField] private RectTransform _pivotRect;
        [SerializeField] private Image _icon;
        [SerializeField] private string _uiId = "ui_barracks_spell_description";
        private string _heroId;
        private HeroInfo _heroInfo;
        private GameObject _heroGo;
    }
}