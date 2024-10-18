using RobotCastle.Battling;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BarracksMainSpellDescriptionUI : DescriptionUI
    {
        public override void Show(GameObject source)
        {
            _provider = source.GetComponent<BarracksHeroSpellView>();
            _rect.transform.parent = _provider.pivotRect.parent;
            _rect.anchorMin = _provider.pivotRect.anchorMin;
            _rect.anchorMax = _provider.pivotRect.anchorMax;
            _rect.anchoredPosition = _provider.pivotRect.anchoredPosition;
            var mainSpell = _provider.heroInfo.spellInfo.mainSpellId;
            var db = ServiceLocator.Get<ModifiersDataBase>();
            var spellProvider = (SpellProvider)db.GetSpell(mainSpell);
            
            _descriptionUI.ShowNameAndDescription(spellProvider);
            _animator.FadeIn();
        }

        public override void Hide()
        {
            _animator.FadeOut();
        }
        
        
        [SerializeField] private SpellDescriptionUI _descriptionUI;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private FadeInOutAnimator _animator;
        private BarracksHeroSpellView _provider;
    }
}