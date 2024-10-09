using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Summoning
{
    public class CardsOutputDisplay : MonoBehaviour, IScreenUI
    {
        [SerializeField] private bool _logAllOnStart;
        [SerializeField] private float _sizePerRow;
        [SerializeField] private float _splashFadeStart = 1f;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private ProperButton _skipBtn;
        [SerializeField] private ProperButton _exitBtn;
        [SerializeField] private TextMeshProUGUI _textCount;
        [Space(5)]
        [SerializeField] private Card _heroCard;
        [SerializeField] private Animator _heroCardAnimator;
        [SerializeField] private float _heroCardDelay;
        [Space(5)]
        [SerializeField] private Card _showCard;
        [SerializeField] private Animator _showAnimator;
        [Space(5)]
        [SerializeField] private float _fadeTime;
        [SerializeField] private Image _fadeImage;
        [SerializeField] private List<GameObject> _firstObjects;
        [Space(5)]
        [SerializeField] private List<GameObject> _cardsOneByOneObjects;
        [SerializeField] private List<GameObject> _gridObjects;
        [Space(5)]
        [SerializeField] private Pool _pool;
        private readonly List<Card> _active = new(10);
        private bool _waiting;
        
        
        public async Task Show(List<SummonOutput> outputs, CancellationToken token)
        {
            if (outputs.Count == 0)
            {
                CLog.LogError($"zero elements !!!");
                return;
            }
            SummoningManager.SetHeroAsFirst(outputs);
            _skipBtn.gameObject.SetActive(false);
            _exitBtn.gameObject.SetActive(false);
            gameObject.SetActive(true);

            if (_logAllOnStart)
            {
                var str = $"[{nameof(CardsOutputDisplay)}] Displaying {outputs.Count} objects: \n";
                foreach (var output in outputs)
                    str += $"Type: {output.data.type}, Id: {output.data.id}\n";
                CLog.Log(str);       
            }
            
            foreach (var card in _active) { _pool.Return(card); }
            _active.Clear();

            var heroesDb = ServiceLocator.Get<HeroesDatabase>();
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            var descrDb = ServiceLocator.Get<DescriptionsDataBase>();
            
            var height = _sizePerRow * (Mathf.RoundToInt(outputs.Count));
            var size = _rect.sizeDelta;
            size.y = height;
            _rect.sizeDelta = size;
            var countLeft = outputs.Count;
            foreach (var go in _gridObjects) { go.SetActive(false); }
            foreach (var go in _cardsOneByOneObjects) { go.SetActive(false); }
            var startIndex = 0;
            var hasHero = outputs[0].data.type == SummonConfig.Id_NewHero;
            if (hasHero)
            {
                CLog.Log($"New hero{outputs[0].data.id}");
                foreach (var go in _firstObjects) { go.SetActive(false); }
                Setup(_heroCard, outputs[0], viewDb, heroesDb, descrDb);
                startIndex = 1;
                _heroCardAnimator.gameObject.SetActive(true);
                _heroCardAnimator.Play("Show Hero", 0, 0);
                await Task.Delay((int)(_heroCardDelay * 1000), token);
                if (token.IsCancellationRequested) { return; }
                countLeft--;
            }
            else
            {
                _skipBtn.gameObject.SetActive(true);
                _heroCardAnimator.gameObject.SetActive(false);
                foreach (var go in _firstObjects) { go.SetActive(true); }
            }
            _waiting = true;
            _skipBtn.gameObject.SetActive(true);
            while (_waiting && !token.IsCancellationRequested){ await Task.Yield(); }
            if (token.IsCancellationRequested) { return; }
            
            if (countLeft <= 0)
            {
                _waiting = true;
                while (_waiting && !token.IsCancellationRequested){ await Task.Yield(); }
                if (token.IsCancellationRequested) { return; }
                Hide();
                return;
            }
            
            if (hasHero) { _heroCardAnimator.gameObject.SetActive(false); }
            foreach (var go in _firstObjects) { go.SetActive(false); }
            foreach (var go in _cardsOneByOneObjects) { go.SetActive(true); }
            // first is separate for animation
            _textCount.text = countLeft.ToString();
            Setup(_showCard, outputs[startIndex], viewDb, heroesDb, descrDb);
            _showAnimator.Play("Show", 0, 0f);
            _fadeImage.gameObject.SetActive(true);
            SplashAnimate();
            await Task.Delay((int)(1000 * _fadeTime), token);
            _textCount.text = countLeft.ToString();
            countLeft--;
            
            _waiting = true;
            for (var i = startIndex; i < outputs.Count; i++)
            {
                while (_waiting && !token.IsCancellationRequested){ await Task.Yield(); }
                if (token.IsCancellationRequested) { return; }
                // var output = outputs[i];
                _waiting = true;
                Setup(_showCard, outputs[i], viewDb, heroesDb, descrDb);
                _showAnimator.Play("Show", 0, 0f);
                SplashAnimate();
                _textCount.text = countLeft.ToString();
                countLeft--;
            }

            while (_waiting && !token.IsCancellationRequested){ await Task.Yield(); }
            if (token.IsCancellationRequested) { return; }

            _skipBtn.gameObject.SetActive(false);
            _exitBtn.gameObject.SetActive(true);
            foreach (var go in _cardsOneByOneObjects) { go.SetActive(false); }
            foreach (var go in _gridObjects) { go.SetActive(true); }
            
            foreach (var output in outputs)
            {
                var ui = _pool.GetOne() as Card;
                _active.Add(ui);
                Setup(ui, output, viewDb, heroesDb, descrDb);
                if (token.IsCancellationRequested) { return; }
            }
            
            _waiting = true;
            while (_waiting && !token.IsCancellationRequested){ await Task.Yield(); }
            if (token.IsCancellationRequested) { return; }
            Hide();
            
            void SplashAnimate()
            {
                _fadeImage.DOKill();
                _fadeImage.SetAlpha(_splashFadeStart);
                _fadeImage.DOFade(0f, _fadeTime);
            }
        }

        private void Setup(Card ui, SummonOutput output, 
            ViewDataBase viewDb, HeroesDatabase heroesDb, DescriptionsDataBase descrDb)
        {
            switch (output.data.type)
            {
                case SummonConfig.Id_NewHero: {
                    var info = heroesDb.GetHeroViewInfo(output.data.id);
                    ui.SetIcon(ViewDataBase.GetHeroSprite(info.iconId));  
                    ui.SetTitleAndCount(info.name, $"+{output.data.level}");
                }
                    break;
                case SummonConfig.Id_OwnedHero: {
                    var info = heroesDb.GetHeroViewInfo(output.data.id);
                    ui.SetIcon(ViewDataBase.GetHeroSprite(info.iconId));        
                    ui.SetTitleAndCount(info.name, $"+{output.data.level}");
                }
                    break;
                case SummonConfig.Id_Gold or SummonConfig.Id_AnyInventoryItem: {
                    ui.SetIcon(viewDb.GetGeneralItemSprite(output.data.id));
                    if (descrDb.descriptions.ContainsKey(output.data.id) == false)
                        CLog.LogError($"Descriptions db does not contain: {output.data.id}");    
                    else
                        ui.SetTitleAndCount(descrDb.descriptions[output.data.id].parts[0], $"+{output.data.level}");
                }
                    break;
                default:
                    CLog.LogError($"[{nameof(CardsOutputDisplay)}] Don't know how to display SummonOutput: {output.data.type}");
                    break;
            }
            ui.PoolShow();
            ui.transform.SetSiblingIndex(ui.transform.childCount - 1);
        }

        public void Hide()
        {
            CLog.Log($"[{nameof(CardsOutputDisplay)}] Closing");
            _waiting = false;
            gameObject.SetActive(false);
        }

        private void OnBtn()
        {
            CLog.Log($"[{nameof(CardsOutputDisplay)}] Next cards step");
            _waiting = false;
        }

        private void OnEnable()
        {
            _skipBtn.OnDown += OnBtn;
            _exitBtn.OnDown += OnBtn;
        }

        private void OnDisable()
        {
            _skipBtn.OnDown -= OnBtn;
            _exitBtn.OnDown -= OnBtn;
        }
    }
}