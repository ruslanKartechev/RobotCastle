using System.Threading;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellDeathSentence : Spell, IFullManaListener, IHeroProcess
    {
        public SpellDeathSentence(SpellConfigDeathSentence config, HeroComponents components)
        {
            _config = config;
            _components = components;
            Setup(config, out var manaAdder);
        }

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            Working(_token.Token);
        }

        public void Stop()
        {
            if(_isActive)
            {
                _token?.Cancel();
                _isActive = false;
                if(_fxView != null)
                    _fxView.gameObject.SetActive(false);
            }
        }
        
        private SpellConfigDeathSentence _config;
        private CancellationTokenSource _token;
        private SpellParticlesByLevel _fxView;
        
        private async void Working(CancellationToken token)
        {
            var hero = _components.GetComponent<IHeroController>();
            hero.StopCurrentBehaviour();
            _components.stats.ManaResetAfterFull.Reset(_components);
            while (!token.IsCancellationRequested)
            {
                _components.heroUI.ManaUI.AnimateTimedSpell(0f, 1f, _config.manaGain);
                await HeroesManager.WaitGameTime(_config.manaGain, token);
                if (token.IsCancellationRequested)
                    return;
                _components.animator.Play("Cast", 0, 0);
                GetFxView().PlayLevelAtPoint(_components.transform.position, 0);
                var enemies = HeroesManager.GetHeroesEnemies(_components);
                for (var i = enemies.Count-1; i >= 0; i--)
                {
                    var en = enemies[i];
                    _components.damageSource.DamageSpellAndPhys(en.Components.damageReceiver);
                }
            }
        }
        
              
        private SpellParticlesByLevel GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_DeathSentence);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticlesByLevel>();
            }
            return _fxView;
        }
    }

}