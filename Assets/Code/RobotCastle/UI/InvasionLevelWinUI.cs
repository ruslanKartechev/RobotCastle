using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.MainMenu;
using RobotCastle.Saving;
using SleepDev.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class InvasionLevelWinUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private List<FadeHighlightItem> _rewardItems;
        [SerializeField] private List<FadeHighlightItem> _heroes;
        [SerializeField] private TextMeshProUGUI _textLvlTitle;
        [SerializeField] private TextMeshProUGUI _textChapter;
        [SerializeField] private TextMeshProUGUI _textPlayerEnegy;
        [SerializeField] private MyButton _btnPlayAgain;
        [SerializeField] private MyButton _btnReturn;
        [SerializeField] private Image _xpProgBar;
        [SerializeField] private TextMeshProUGUI _playerLevelText;
        [Space(10)] 
        [SerializeField] private float _xpFillTime = .5f;
        [SerializeField] private float _levelPunchScale = .2f;
        [SerializeField] private float _levelPunchTime = 0.25f;
        [SerializeField] private float _itemPunchTime = 1.2f;
        [SerializeField] private float _itemPunchScale = -0.2f;
        [SerializeField] private float _itemsPunchDelay = 0.12f;
        [SerializeField] private float _buttonsAnimationDelay = 0.3f;
        [SerializeField] private FadeInOutAnimator _fadeAnimatorItems;
        [SerializeField] private FadeInOutAnimator _fadeAnimatorButtons;
        private InvasionWinArgs _args;
        private bool _inputActive;
        
        public void Show(InvasionWinArgs args)
        {
            _args = args;
            var playerData = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            var chaptersDb = ServiceLocator.Get<ProgressionDataBase>();
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            _textLvlTitle.text = chaptersDb.chapters[args.selectionData.chapterIndex].viewName;
            _textChapter.text = $"Chapter: {args.selectionData.chapterIndex + 1}";
            _textPlayerEnegy.text = $"{playerData.playerEnergy}/{playerData.playerEnergyMax}";

            for (var i = 0; i < _heroes.Count; i++)
            {
                _heroes[i].SetTextCount($"EXP+{Mathf.RoundToInt(args.heroesXp[i])}");
            }

            var rewardsCount = args.rewards.Count;
            for (var i = 0; i < _rewardItems.Count; i++)
            {
                if (i < rewardsCount)
                {
                    var reward = args.rewards[i];
                    _rewardItems[i].gameObject.SetActive(true);
                    _rewardItems[i].SetTextCount($"+{reward.level}");
                    _rewardItems[i].SetIcon(viewDb.GetGeneralItemSprite(reward.id));
                }
                else
                {
                    _rewardItems[i].gameObject.SetActive(false);
                }
            }
            _btnPlayAgain.AddMainCallback(Replay);
            _btnReturn.AddMainCallback(Return);
            StartCoroutine(FillingProgressXPBar(args));
            StartCoroutine(Animating());
            _inputActive = true;
        }

        private IEnumerator Animating()
        {
            var delay = _itemsPunchDelay;
            for (var i = 0; i < _rewardItems.Count; i++)
            {
                var it = _rewardItems[i].transform;
                if (it.gameObject.activeSelf == false)
                    continue;
                it.DOPunchScale(Vector3.one * _itemPunchScale, _itemPunchTime).SetDelay(delay).SetEase(Ease.OutBack);
            }
            _fadeAnimatorItems.On();
            _fadeAnimatorItems.FadeIn();
            _fadeAnimatorButtons.Off();
            yield return new WaitForSeconds(_buttonsAnimationDelay);
            _fadeAnimatorButtons.On();
            _fadeAnimatorButtons.FadeIn();
        }
        
        private IEnumerator FillingProgressXPBar(InvasionWinArgs args)
        {
            var playerData = DataHelpers.GetPlayerData();
            var xpManager = ServiceLocator.Get<CastleXpManager>();
            if (args.playerNewLevelReached)
            {
                _playerLevelText.text = $"{playerData.playerLevel}";
                yield return StartCoroutine(Filling(.8f, 1f, _xpFillTime)); 
                yield return null;
                _playerLevelText.text = $"{playerData.playerLevel+1}";
                _playerLevelText.transform.DOPunchScale(Vector3.one * _levelPunchScale, _levelPunchTime);
                var t2 = xpManager.GetProgressToNextLvl();
                _xpProgBar.fillAmount = 0f;
                yield return new WaitForSeconds(_levelPunchTime);
                yield return null;
                yield return StartCoroutine(Filling(0f, t2, _xpFillTime));
                
            }
            else
            {
                _playerLevelText.text = $"{playerData.playerLevel+1}";
                var t1 = (playerData.playerXp - args.playerXpAdded) / xpManager.GetMaxXp();
                var t2 = xpManager.GetProgressToNextLvl();
                yield return StartCoroutine(Filling(t1, t2, _xpFillTime));
            }
        }

        private IEnumerator Filling(float from, float to, float time)
        {
            var elapsed = 0f;
            var t = 0f;
            while (t < 1)
            {
                _xpProgBar.fillAmount = Mathf.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            _xpProgBar.fillAmount = to;
        }
        
        private void Replay()
        {
            if (!_inputActive) return;
            _inputActive = false;
            _args.replayCallback?.Invoke();
        }

        private void Return()
        {
            if (!_inputActive) return;
            _inputActive = false;
            _args.returnCallback?.Invoke();

        }
        
    }
    
}