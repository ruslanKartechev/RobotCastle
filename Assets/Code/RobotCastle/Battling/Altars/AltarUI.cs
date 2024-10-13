using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.UI;
using TMPro;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    public class AltarUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private List<AltarPointsHighlight> _levelHighlights;
        [SerializeField] private List<AltarModifierUI> _modifiersUI;
        [SerializeField] private MyButton _lvlUpBtnOne;
        [SerializeField] private MyButton _lvlUpBtnMultiple;
        private Altar _altar;


        public void Init(Altar altar)
        {
            _nameText.text = altar.ViewName;
            ServiceLocator.Get<AltarManager>().SetupData();
            _lvlUpBtnOne.AddMainCallback(PurchaseOne);
            _lvlUpBtnMultiple.AddMainCallback(PurchaseMultiple);
            for (var i = 0; i < _modifiersUI.Count; i++)
                _modifiersUI[i].Init(_altar.modifiers[i]);
            SetPointsViewAndBtnState(altar.GetPoints());
        }

        public void SetPointsViewAndBtnState(int playerFreePoints)
        {
            SetPointsView(playerFreePoints);
            SetButtonsStateBasedOnPoints(playerFreePoints);
        }

        public void SetPointsView(int playerFreePoints)
        {
            var points = playerFreePoints;
            if (points == 0)
            {
                foreach (var highlight in _levelHighlights)
                    highlight.SetState(false);
                return;
            }
            for (var i = 0; i < _levelHighlights.Count; i++)
                _levelHighlights[i].SetState(i < points);
            
        }

        public void SetButtonsStateBasedOnPoints(int playerFreePoints)
        {
            if(playerFreePoints == 0)
                SetButtonsInteractable(false);
            else
                SetButtonInteractable();
        }        

        public void SetButtonInteractable()
        {
            if (_altar.GetPoints() >= Altar.MaxPoints)
                SetButtonsInteractable(false);
            else
                SetButtonsInteractable(true);
        }

        public void ZeroPoints()
        {
            _altar.SetPoints(0);
            foreach (var highlight in _levelHighlights)
                highlight.SetState(false);
            ServiceLocator.Get<AltarManager>().RemoveAllPointFromAltar(_altar);
        }
        
        private void PurchaseOne()
        {
            if (_altar.GetPoints() >= Altar.MaxPoints)
                return;
            ServiceLocator.Get<AltarManager>().AddPointToAltar(_altar);
        }

        private void PurchaseMultiple()
        {
            if (_altar.GetPoints() >= Altar.MaxPoints)
                return;
            ServiceLocator.Get<AltarManager>().AddPointsToAltar(_altar, 5);
        }

        public void SetButtonsInteractable(bool interactable)
        {
            _lvlUpBtnOne.SetInteractable(interactable);
            _lvlUpBtnMultiple.SetInteractable(interactable);
        }

    }
}