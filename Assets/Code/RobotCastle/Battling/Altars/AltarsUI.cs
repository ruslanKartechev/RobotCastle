using System;
using System.Collections.Generic;
using RobotCastle.UI;
using UnityEngine;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using TMPro;


namespace RobotCastle.Battling.Altars
{
    public class AltarsUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private List<AltarUI> _uiAltars;
        [SerializeField] private TextMeshProUGUI _pointsAvailable;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private TextMeshProUGUI _requiredLevelText;
        [SerializeField] private MyButton _btnPurchasePoint;
        [SerializeField] private MyButton _btnRetrievePoints;
        [SerializeField] private MyButton _BtnClose;
        

        private AltarsDatabase _db;
        private AltarsSave _save;
        
        public void Show()
        {
            _save = DataHelpers.GetAltarsSave();
            _db = ServiceLocator.Get<AltarsDatabase>();
            ServiceLocator.Get<AltarManager>().SetupData();
            for (var i = 0; i < _uiAltars.Count; i++)
                _uiAltars[i].Init(_db.GetAltar(i));
            _btnRetrievePoints.AddMainCallback(RetrievePoints);
            _btnPurchasePoint.AddMainCallback(TryPurchasePoint);
            _BtnClose.AddMainCallback(Close);
            _pointsAvailable.text = _save.pointsFree.ToString();
            UpdateButtonsState();
        }

        private void Close()
        {
            gameObject.SetActive(false);
            ServiceLocator.Get<IUIManager>().OnClosed(UIConstants.UIAltars);
        }

        private void RetrievePoints()
        {
            ServiceLocator.Get<AltarManager>().RemoveAllPointsFromAllAltars();
            for (var i = 0; i < _uiAltars.Count; i++)
            {
                _uiAltars[i].SetPointsViewAndBtnState(_save.pointsFree);
            }
        }

        private void TryPurchasePoint()
        {
            var did = ServiceLocator.Get<AltarManager>().TryPurchaseOneFreePoint();
            var free = _save.pointsFree;
            if (did)
            {
                _pointsAvailable.text = free.ToString();
                for (var i = 0; i < _uiAltars.Count; i++)
                    _uiAltars[i].SetButtonsStateBasedOnPoints(free);
            }
        }

        private void UpdateButtonsState()
        {
            var player = DataHelpers.GetPlayerData();
            if(_db.HasReachedMaxPoints(_save.pointsTotal))
            {
                CLog.Log($"Reached max altar points!");
                _btnPurchasePoint.gameObject.SetActive(false);
                _costText.gameObject.SetActive(false);
                _requiredLevelText.gameObject.SetActive(false);
                return;
            }
            var lvl = player.playerLevel + 1;
            if (_db.CanBuyMorePoints(_save.pointsTotal, lvl))
            {
                _costText.text = ((int)_db.GetNextPointCost(lvl)).ToString();
                _costText.gameObject.SetActive(true);
                _requiredLevelText.gameObject.SetActive(false);
            }
            else
            {
                _costText.gameObject.SetActive(false);
                _requiredLevelText.gameObject.SetActive(true);
                _requiredLevelText.text = $"Requires level: {lvl}";
            }

        }
    }
}