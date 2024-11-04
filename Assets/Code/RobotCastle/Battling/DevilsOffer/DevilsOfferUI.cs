using System;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using TMPro;
using UnityEngine;

namespace RobotCastle.Battling.DevilsOffer
{
    public class DevilsOfferUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private MyButton _btnAccept;
        [SerializeField] private MyButton _btnDecline;
        [SerializeField] private TextMeshProUGUI _penaltyText;
        [SerializeField] private ItemDescriptionLongUI _itemDescription;
        [SerializeField] private BlackoutFadeScreen _fadeScreen;
        [SerializeField] private TextMeshProUGUI _tierText;

        private Action<bool> _callback;

        public void Show(DevilsOfferData offerData, int level, Action<bool> callback)
        {
            gameObject.SetActive(true);
            _fadeScreen.FadeInWithId(UIConstants.UIDevilsOffer);
            _callback = callback;
            _btnAccept.AddMainCallback(Accept);
            _btnDecline.AddMainCallback(Decline);
            _btnAccept.SetInteractable(true);
            _btnDecline.SetInteractable(true);
            var tierText = "";
            switch (level)
            {
                case 0: tierText = "difficult";
                    break;
                case 1: tierText = "hard";
                    break;
                case 2: tierText = "very hard";
                    break;
                case 3: tierText = "super hard";
                    break;
                default: tierText = "hard";
                    break;
            }
            _tierText.text = tierText;
            
            
            var reward = offerData.reward;
            switch (reward.type)
            {
                case MergeConstants.TypeWeapons:
                    var heroItemdata = HeroWeaponData.GetDataWithDefaultModifiers(reward);
                    _itemDescription.ShowItem(heroItemdata);
                    break;
                case MergeConstants.TypeBonus:
                    _itemDescription.ShowBonus(reward);
                    break;
            }
    
            var penaltyText = "";
            switch (offerData.penaltyType)
            {
                case EDevilsPenaltyType.CastleDurability:
                    penaltyText = $"Reduce castle durability by {HeroesManager.Red((int)offerData.penaltyValue)}";
                    break;
                case EDevilsPenaltyType.AdditionalEnemyForces:
                    penaltyText = $"+{HeroesManager.Red(Mathf.RoundToInt(offerData.penaltyValue * 100))}% additional enemy forces";
                    break;
                case EDevilsPenaltyType.HigherEnemyTier:
                    penaltyText = $"+{HeroesManager.Red(Mathf.RoundToInt(offerData.penaltyValue))} enemy tier upgrade!";
                    break;
            }
            _penaltyText.text = penaltyText;
            
        
        }

        private void Accept()
        {
            _callback?.Invoke(true);
            _fadeScreen.FadeOut();   
        }
        
        private void Decline()
        {
            _callback?.Invoke(false);
            _fadeScreen.FadeOut();
        }
        
    }
}