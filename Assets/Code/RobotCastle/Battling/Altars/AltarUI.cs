using System;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    public class AltarUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _pointsCountText;
        [SerializeField] private List<AltarPointsHighlight> _levelHighlights;
        [SerializeField] private List<AltarModifierUI> _modifiersUI;
        [SerializeField] private MyButton _lvlUpBtnOne;
        [SerializeField] private MyButton _lvlUpBtnMultiple;
        private Altar _altar;


        public void Init(Altar altar)
        {
            _altar = altar;
            _nameText.text = altar.ViewName;
            ServiceLocator.Get<AltarManager>().SetupData();
            _lvlUpBtnOne.AddMainCallback(PurchaseOne);
            _lvlUpBtnMultiple.AddMainCallback(PurchaseMultiple);
            for (var i = 0; i < _modifiersUI.Count; i++)
                _modifiersUI[i].Init(altar.modifiers[i]);
            altar.OnPointsUpdated -= OnPointsUpdated;
            altar.OnPointsUpdated += OnPointsUpdated;
        }

        private void OnDisable()
        {
            if (_altar != null)
            {
                _altar.OnPointsUpdated -= OnPointsUpdated;
            }
        }

        public void SetPointsViewAndBtnState(int altarPoints, int playerFreePoints)
        {
            _pointsCountText.text = altarPoints.ToString();
            SetPointsView(altarPoints);
            SetInteractableOnFreePoints(playerFreePoints);
        }

        public void SetPointsView(int altarPoints)
        {
            if (altarPoints == 0)
            {
                foreach (var highlight in _levelHighlights)
                    highlight.SetState(false);
                return;
            }
            for (var i = 0; i < _levelHighlights.Count; i++)
                _levelHighlights[i].SetState(i < altarPoints);
            
        }

        public void SetInteractableOnFreePoints(int playerFreePoints)
        {
            if(playerFreePoints == 0)
                SetButtonsInteractable(false);
            else
                SetButtonsInteractableIfNotMaxTier();
        }        

        public void SetButtonsInteractableIfNotMaxTier()
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
        
        private void OnPointsUpdated(int prevVal, int newVal)
        {
            CLog.Log($"[OnPointsUpdated][{gameObject.name}] new val {newVal}");
            if (prevVal != newVal)
            {
                SetPointsAndAnimate(newVal);
                if (newVal > prevVal)
                {
                    for (var i = prevVal; i < newVal; i++)
                        _levelHighlights[i].AnimateOn();
                }
                else
                {
                    for (var i = prevVal - 1; i >= newVal; i--)
                        _levelHighlights[i].SetState(false);
                }
            }
        }

        private void SetPointsAndAnimate(int newVal)
        {
            _pointsCountText.text = newVal.ToString();
            _pointsCountText.transform.DOPunchScale(Vector3.one * -.1f, .22f);
        }
        
        private void PurchaseOne()
        {
            if (_altar.GetPoints() >= Altar.MaxPoints)
                return;
            var did = ServiceLocator.Get<AltarManager>().AddPointToAltar(_altar);
        }

        private void PurchaseMultiple()
        {
            if (_altar.GetPoints() >= Altar.MaxPoints)
                return;
            var did = ServiceLocator.Get<AltarManager>().AddPointsToAltar(_altar, 5);
        }

        public void SetButtonsInteractable(bool interactable)
        {
            _lvlUpBtnOne.SetInteractable(interactable);
            _lvlUpBtnMultiple.SetInteractable(interactable);
        }

    }
}