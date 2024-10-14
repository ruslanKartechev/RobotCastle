using System;
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
        [SerializeField] private FadeInOutAnimator _fadeAnimator;
        private Action<bool> _callback;

        public void Show(DevilsOfferData offerData, Action<bool> callback)
        {
            _fadeAnimator.On();
            _fadeAnimator.FadeIn();
            gameObject.SetActive(true);
            _callback = callback;
            _btnAccept.AddMainCallback(Accept);
            _btnDecline.AddMainCallback(Decline);
            _btnAccept.SetInteractable(true);
            _btnDecline.SetInteractable(true);
            
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
                    penaltyText = $"Reduce castle durability by {Red(offerData.penaltyValue)}";
                    break;
                case EDevilsPenaltyType.AdditionalEnemyForces:
                    penaltyText = $"+{Red(Mathf.RoundToInt(offerData.penaltyValue * 100))}% additional enemy forces";
                    break;
                case EDevilsPenaltyType.HigherEnemyTier:
                    penaltyText = $"+{Red(Mathf.RoundToInt(offerData.penaltyValue))} enemy tier upgrade!";
                    break;
            }
            _penaltyText.text = penaltyText;
            
            string Red(float msg)
            {
                return $"<color=#FF1111>{msg}</color>";
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void Accept()
        {
            _callback?.Invoke(true);   
            Hide();
        }
        
        private void Decline()
        {
            _callback?.Invoke(false);               
            Hide();
        }
        
    }
}