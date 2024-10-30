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
        [SerializeField] private RectTransform _rectRewards;
        [SerializeField] private RectTransform _rectHeroes;
        [SerializeField] private RectTransform _rectXp;
        [SerializeField] private RectTransform _rectButtons;
        [Space(10)]
        [SerializeField] private List<FadeHighlightItem> _rewardItems;
        [SerializeField] private List<FadeHighlightItem> _heroes;
        [SerializeField] private TextMeshProUGUI _textLvlTitle;
        [SerializeField] private TextMeshProUGUI _textChapter;
        [SerializeField] private TextMeshProUGUI _textPlayerEnegy;
        [SerializeField] private MyButton _btnPlayAgain;
        [SerializeField] private MyButton _btnReturn;
        [SerializeField] private Image _xpProgBar;
        [SerializeField] private TextMeshProUGUI _playerLevelText;
        [SerializeField] private TextMeshProUGUI _addedXpText;
        [Space(10)] 
        [SerializeField] private float _xpFillTime = .5f;
        [SerializeField] private float _levelPunchScale = .2f;
        [SerializeField] private float _levelPunchTime = 0.25f;
        [Space(10)]
        [SerializeField] private Ease _scaleDownEase;
        [SerializeField] private float scaleUp = 1.4f;
        [SerializeField] private float scaleTime = .31f;
        [SerializeField] private float scaleDownDelay = 0.41f;
        [Space(10)]
        [SerializeField] private Color _backColorMain;
        [SerializeField] private Color _backColorBright;
        [SerializeField] private float _colorFadeTime = .3f;
        [SerializeField] private Image _backgroundImage;
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
            
            StartCoroutine(Animating(args));
        }

        private IEnumerator Animating(InvasionWinArgs args)
        {
            _inputActive = true;

            _rectRewards.gameObject.SetActive(false);
            _rectHeroes.gameObject.SetActive(false);
            _rectXp.gameObject.SetActive(false);
            _rectButtons.gameObject.SetActive(false);
    
            
            _rectRewards.gameObject.SetActive(true);
            _rectRewards.localScale = Vector3.one * scaleUp;
            yield return new WaitForSeconds(scaleDownDelay);
            _rectRewards.DOScale(Vector3.one, scaleTime).SetEase(_scaleDownEase);
            yield return new WaitForSeconds(scaleTime);
            FadeAnimate();

            _rectHeroes.gameObject.SetActive(true);
            _rectHeroes.localScale = Vector3.one * scaleUp;
            yield return new WaitForSeconds(scaleDownDelay);
            _rectHeroes.DOScale(Vector3.one, scaleTime).SetEase(_scaleDownEase);
            yield return new WaitForSeconds(scaleTime);
            FadeAnimate();

            _rectXp.gameObject.SetActive(true);
            _rectXp.localScale = Vector3.one * scaleUp;
            StartCoroutine(FillingProgressXPBar(args));
            yield return new WaitForSeconds(scaleDownDelay);
            _rectXp.DOScale(Vector3.one, scaleTime).SetEase(_scaleDownEase);
            yield return new WaitForSeconds(scaleTime);
            FadeAnimate();

            _rectButtons.gameObject.SetActive(true);
            _rectButtons.localScale = Vector3.one * scaleUp;
            yield return new WaitForSeconds(scaleDownDelay);
            _rectButtons.DOScale(Vector3.one, scaleTime).SetEase(_scaleDownEase);
            _inputActive = true;

        }

        private void FadeAnimate()
        {
            _backgroundImage.DOKill();
            _backgroundImage.color = _backColorBright;
            _backgroundImage.DOColor(_backColorMain, _colorFadeTime);
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
                _addedXpText.text = $"+{args.playerXpAdded}";
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