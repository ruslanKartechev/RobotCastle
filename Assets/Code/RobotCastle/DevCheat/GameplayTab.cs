using System;
using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Testing;
using RobotCastle.UI;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    public class GameplayTab : Tab
    {
        
        public override void Show(Action closeCallback)
        {
            _closedCallback = closeCallback;
            _btnNextHero.AddMainCallback(NextHero);   
            _btnPrevHero.AddMainCallback(PrevHero);
            _btnNextLvl.AddMainCallback(NextLvl);
            _btnPrevLvl.AddMainCallback(PrevLvl);
            _btnSpawnHero.AddMainCallback(SpawnHero);
            _btnWin.AddMainCallback(Win);
            _btnFail.AddMainCallback(Fail);
            _btnReturn.AddMainCallback(Return);
            _btnAddMoney.AddMainCallback(AddMoney);
            gameObject.SetActive(true);
            _addMoneyText.text = $"+{_addedMoney}";
            
            _textLevel.text = (_lvl + 1).ToString();     
            var id = _heroOptions[_heroInd];
            _textHero.text = id;
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            _isSceneCorrect = scene == GlobalConfig.SceneBattle;
            CLog.Log($"Scene: {scene}. Correct? {_isSceneCorrect}");
            
            
        }

        private void AddMoney()
        {
            ServiceLocator.Get<GameMoney>().AddMoney(_addedMoney);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
        
        [SerializeField] private MyButton _btnNextHero;
        [SerializeField] private MyButton _btnPrevHero;
        [SerializeField] private MyButton _btnSpawnHero;
        [SerializeField] private TextMeshProUGUI _textHero;
        [SerializeField] private List<string> _heroOptions;
        [Space(10)]
        [SerializeField] private MyButton _btnNextLvl;
        [SerializeField] private MyButton _btnPrevLvl;
        [SerializeField] private TextMeshProUGUI _textLevel;
        [Space(10)]
        [SerializeField] private MyButton _btnFail;
        [SerializeField] private MyButton _btnWin;
        [SerializeField] private MyButton _btnReturn;
        [Space(10)]
        [SerializeField] private MyButton _btnAddMoney;
        [SerializeField] private TextMeshProUGUI _addMoneyText;
        [SerializeField] private int _addedMoney;
        private Action _closedCallback;
        private bool _isSceneCorrect;
        private int _lvl;
        private int _heroInd;

        private void Return()
        {
            Close();
            _closedCallback?.Invoke();
        }

        private void Win()
        {
            if (!_isSceneCorrect)
                return;
            var level = FindObjectOfType<BattleLevel>();
            if (level == null)
            {
                CLog.LogRed("[Cheat] BattleLevel object not found");
                return;
            }
            level.ForceWin();
        }

        private void Fail()
        {
            if (!_isSceneCorrect)
                return;
            var level = FindObjectOfType<BattleLevel>();
            if (level == null)
            {
                CLog.LogRed("[Cheat] BattleLevel object not found");
                return;
            }
            level.ForceFail();
        }

        private void NextLvl()
        {
            _lvl++;
            _lvl = Mathf.Clamp(_lvl, 0, 6);
            _textLevel.text = (_lvl + 1).ToString();     
        }

        private void PrevLvl()
        {
            _lvl--;
            _lvl = Mathf.Clamp(_lvl, 0, 6);
            _textLevel.text = (_lvl + 1).ToString();     
        }

        private void NextHero()
        {
            _heroInd++;
            _heroInd = Mathf.Clamp(_heroInd, 0, _heroOptions.Count - 1);
            var id = _heroOptions[_heroInd];
            _textHero.text = id;            
        }

        private void PrevHero()
        {
            _heroInd--;
            _heroInd = Mathf.Clamp(_heroInd, 0, _heroOptions.Count - 1);
            var id = _heroOptions[_heroInd];
            _textHero.text = id;            

        }

        private void SpawnHero()
        {
            if (!_isSceneCorrect) return;
            var level = FindObjectOfType<BattleLevel>();
            if (level == null)
            {
                CLog.LogRed("[Cheat] BattleLevel object not found");
                return;
            }

            var battle = ServiceLocator.Get<Battle>();
            if (battle == null)
            {
                CLog.LogRed("Battle not found");
                return;
            }
            if (battle.State == BattleState.Going)
            {
                CLog.LogRed($"Cannot spawn while battle is going");
                return;
                return;
            }
            var core = new CoreItemData(_lvl, _heroOptions[_heroInd], "unit");
            var spawnItem = CheatItemsSpawner.SpawnHeroOrItem(core, false, default);

        }
    }
}