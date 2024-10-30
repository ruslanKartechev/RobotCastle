using System;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Battling;
using RobotCastle.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class BattleDamageStatsUI : MonoBehaviour
    {
        public enum Mode {Dealt, Received} 

        public void SetCollector(BattleDamageStatsCollector collector)
        {
            _collector = collector;
            SubToEvents(true);
        }

        public void SubToEvents(bool sub)
        {
            if (sub == _didSub)
                return;
            _didSub = true;
            _collector.onListUpdated += OnListSet;
            _collector.onUpdated += OnUpdated;
        }
 
        public void SetInteractable(bool allowed)
        {
            if(_isOpened && !allowed)
                Close();
            _inputAllowed = allowed;
        }

        public void SetModeDealtDamage()
        {
            _mode = Mode.Dealt;
            _highlightDealt.enabled = true;
            _highlightReceived.enabled = false;
            OnUpdated();
        }

        public void SetModeReceivedDamage()
        {
            _mode = Mode.Received;
            _highlightDealt.enabled = false;
            _highlightReceived.enabled = true;
            OnUpdated();
        }

        private void Start()
        {
            _closeBtn.AddMainCallback(Close);
            _openBtn.AddMainCallback(Open);
            _dealtModeBtn.AddMainCallback(SetModeDealtDamage);
            _receivedModeBtn.AddMainCallback(SetModeReceivedDamage);
            _uiPool.Init();
            _closedGo.SetActive(true);
            _openedGo.SetActive(false);
        }

        private void OnListSet()
        {
            // CLog.LogYellow("OnListSet =======");
            foreach (var perHero in _perHeroUI)
            {
                _uiPool.Return(perHero);   
            }
            _perHeroUI.Clear();
            var count = _collector.data.Count;
            foreach (var (id, data) in _collector.data)
            {
                var ui = _uiPool.GetOne() as PerHeroDamageStatsUI;
                ui.PoolShow();
                ui.InitData(data);
                _perHeroUI.Add(ui);
                ui.UpdateData(_mode);
            }
        }

        private void OnUpdated()
        {
            // CLog.LogYellow("OnUpdated =======");
            if(!_isOpened) return;
            foreach (var ui in _perHeroUI)
                ui.UpdateData(_mode);
        }
        
        private void Open()
        {
            if (!_inputAllowed) return;
            _isOpened = true;
    
            _closedGo.SetActive(false);
            _openedGo.SetActive(true);
            _movingRect.anchoredPosition = _closedPos;
            _movingRect.DOAnchorPos(_openedPos, _moveTime);
            OnUpdated();
        }

        private void Close()
        {
            if (!_inputAllowed) return;
            _isOpened = false;
            _movingRect.DOAnchorPos(_closedPos, _moveTime).OnComplete(() =>
            {
                _closedGo.SetActive(true);
                _openedGo.SetActive(false);
            });
        }
        
        private void OnDisable()
        {
            if (_didSub && _collector != null)
            {
                SubToEvents(false);
            }
        }

        [SerializeField] private Pool _uiPool;
        [Space(10)] 
        [SerializeField] private Image _highlightDealt;
        [SerializeField] private Image _highlightReceived;
        [SerializeField] private MyButton _openBtn;
        [SerializeField] private MyButton _closeBtn;
        [Space(10)]
        [SerializeField] private MyButton _dealtModeBtn;
        [SerializeField] private MyButton _receivedModeBtn;
        [Space(10)] 
        [SerializeField] private GameObject _closedGo;
        [SerializeField] private GameObject _openedGo;
        [SerializeField] private RectTransform _movingRect;
        [SerializeField] private float _moveTime = .5f;
        [SerializeField] private Vector2 _openedPos;
        [SerializeField] private Vector2 _closedPos;

        private bool _isOpened;
        private Mode _mode = Mode.Dealt;
        private List<PerHeroDamageStatsUI> _perHeroUI = new (6);
        private BattleDamageStatsCollector _collector;
        private bool _didSub;
        private bool _inputAllowed = true;
    }
}