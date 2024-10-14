using System;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.UI;
using UnityEngine;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using TMPro;


namespace RobotCastle.Battling.Altars
{
    public class AltarsOverviewUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private List<AltarUI> _uiAltars;
        [SerializeField] private TextMeshProUGUI _pointsAvailable;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private TextMeshProUGUI _requiredLevelText;
        [SerializeField] private GameObject _requirementsObj;
        [SerializeField] private MyButton _btnPurchasePoint;
        [SerializeField] private MyButton _btnRetrievePoints;
        [SerializeField] private MyButton _BtnClose;
        
        private AltarsDatabase _db;
        private AltarsSave _save;
        private AltarManager _manager;

        private void OnDisable()
        {
            if (_manager != null)
                _manager.OnFreePointsCountChanged -= OnFreePointsUpdated;
        }

        public void Show()
        {
            _save = DataHelpers.GetAltarsSave();
            _db = ServiceLocator.Get<AltarsDatabase>();
            _manager = ServiceLocator.Get<AltarManager>();
            _manager.SetupData();
            _manager.OnFreePointsCountChanged -= OnFreePointsUpdated;
            _manager.OnFreePointsCountChanged += OnFreePointsUpdated;
            _pointsAvailable.text = _save.pointsFree.ToString();
            for (var i = 0; i < _uiAltars.Count; i++)
            {
                _uiAltars[i].Init(_db.GetAltar(i));
                _uiAltars[i].SetPointsViewAndBtnState(_save.altars[i].points, _save.pointsFree);
            }
            _btnRetrievePoints.AddMainCallback(RetrievePoints);
            _btnPurchasePoint.AddMainCallback(TryPurchasePoint);
            _BtnClose.AddMainCallback(Close);
            UpdateButtonsState();
        }

        private void OnFreePointsUpdated(int prevVal, int newVal)
        {
            if (prevVal != newVal)
            {
                SetPointsAndAnimate(newVal);
                if (newVal == 0)
                {
                    foreach (var altar in _uiAltars)
                        altar.SetButtonsInteractable(false);
                }
            }
        }
        
        private void SetPointsAndAnimate(int newVal)
        {
            _pointsAvailable.text = newVal.ToString();
            _pointsAvailable.transform.DOPunchScale(Vector3.one * -.1f, .22f);
        }

        private void Close()
        {
            gameObject.SetActive(false);
            ServiceLocator.Get<IUIManager>().OnClosed(UIConstants.UIAltars);
        }

        private void RetrievePoints()
        {
            ServiceLocator.Get<AltarManager>().RemoveAllPointsFromAllAltars();
            var freePoints = _save.pointsFree;
            for (var i = 0; i < _uiAltars.Count; i++)
            {
                _uiAltars[i].ZeroPoints();
                _uiAltars[i].SetInteractableOnFreePoints(freePoints);
            }
        }

        private void TryPurchasePoint()
        {
            var did = ServiceLocator.Get<AltarManager>().TryPurchaseOneFreePoint();
            var free = _save.pointsFree;
            if (did)
            {
                UpdateButtonsState();
                _pointsAvailable.text = free.ToString();
                for (var i = 0; i < _uiAltars.Count; i++)
                    _uiAltars[i].SetInteractableOnFreePoints(free);
            }
        }

        private void UpdateButtonsState()
        {
            var player = DataHelpers.GetPlayerData();
            var canBuy = _manager.CanBuyOnFreePoint();
            var lvl = player.playerLevel + 1;
            var redColor = "#FF1111";
            switch (canBuy)
            {
                case 0: // can
                    CLog.Log($"Can buy more points");
                    _requirementsObj.gameObject.SetActive(false);
                    _btnPurchasePoint.gameObject.SetActive(true);
                    _costText.text = ((int)_db.GetNextPointCost(lvl)).ToString();
                    break;
                case 1: // no money
                    CLog.Log($"Not enough money");
                    _requirementsObj.gameObject.SetActive(false);
                    _btnPurchasePoint.gameObject.SetActive(true);
                    _costText.text = $"<color={redColor}>{(int)_db.GetNextPointCost(lvl)}</color>";
                    break;
                case 2: // Level not mey
                    CLog.Log($"Level not met");
                    _btnPurchasePoint.gameObject.SetActive(false);
                    _requirementsObj.gameObject.SetActive(true);
                    _requiredLevelText.text = $"Requires level <color={redColor}>{lvl}</color>";
                    CLog.LogRed($"Player level: {lvl}, cannot buy points");
                    break;
                case 3: // max points reached
                    CLog.Log($"Max points already");
                    _btnPurchasePoint.gameObject.SetActive(false);
                    _requirementsObj.gameObject.SetActive(false);
                    break;
            }
            
        }
    }
}