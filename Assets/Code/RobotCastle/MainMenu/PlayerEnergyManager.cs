using System;
using System.Collections;
using RobotCastle.Core;
using RobotCastle.Saving;
using SleepDev;
using SleepDev.Data;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class PlayerEnergyManager : MonoBehaviour
    {
        public static PlayerEnergyManager Create()
        {
            var me = new GameObject("energy_manager").AddComponent<PlayerEnergyManager>();
            DontDestroyOnLoad(me.gameObject);
            me._playerData = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            if (me._playerData == null)
            {
                CLog.LogRed("ERROR player data is null ------------");
            }
            me.CheckEnergyDeficit();
            return me;
        }

        /// <summary>
        /// Passes current and max
        /// </summary>
        public event UpdateDelegate<int> OnEnergySet;
        public const int PlayLevelEnergyCost = 60;

        
        public int GetCurrent()
        {
            return _playerData.playerEnergy;
        }

        public int GetMax()
        {
            return _playerData.playerEnergyMax;
        }
        
        public string GetAsStr() => $"{GetCurrent()}/{GetMax()}";

        public void Set(int energy)
        {
            var prev = _playerData.playerEnergy;
            _playerData.playerEnergy = energy;
            OnEnergySet?.Invoke(energy, prev);
        }

        public void Subtract()
        {
            Subtract(PlayLevelEnergyCost);
        }

        public void Subtract(int amount)
        {
            var prev = _playerData.playerEnergy;
            var newVal = prev - amount;
            if (newVal < 0)
                newVal = 0; 
            _playerData.playerEnergy = newVal;
            OnEnergySet?.Invoke(newVal, prev);
            CheckEnergyDeficit();
        }
        
        public void Add(int amount)
        {
            var prev = _playerData.playerEnergy;
            _playerData.playerEnergy = prev + amount;
            OnEnergySet?.Invoke(_playerData.playerEnergy, prev);
            CheckEnergyDeficit();
        }

        public const int EnergyGivenPerOffer = 40;
        private const int SecondsBetweenAddedEnergy = 300;
        private Coroutine _countingDownEnergy;
        private SavePlayerData _playerData;
        
        private PlayerEnergyManager(){}

        private void CheckEnergyDeficit()
        {
            var deficit = _playerData.playerEnergyMax - _playerData.playerEnergy;
            if (deficit > 0)
            {
                if (_playerData.timeWhenOutOfEnergy.IsNull())
                {
                    _playerData.timeWhenOutOfEnergy = DateTimeData.FromNow();
                }
                else
                {
                    var diffSeconds = (DateTime.Now - _playerData.timeWhenOutOfEnergy.GetDateTime()).Seconds;
                    var energyAddedSinceLastTime = diffSeconds / SecondsBetweenAddedEnergy;
                    if (energyAddedSinceLastTime > deficit)
                        energyAddedSinceLastTime = deficit;
                    CLog.Log($"[EnergyManager] SecondsPassed: {diffSeconds}, added energy: {energyAddedSinceLastTime}");
                    _playerData.playerEnergy += energyAddedSinceLastTime;
                    UpdateRestoringTimer();
                }
                if (_playerData.playerEnergyMax > _playerData.playerEnergy)
                    StartEnergyAdding();
            }
            else
            {
            }
        }

        private void UpdateRestoringTimer()
        {
            if (_playerData.playerEnergy < _playerData.playerEnergyMax)
                _playerData.timeWhenOutOfEnergy = DateTimeData.FromNow();
            else
                _playerData.timeWhenOutOfEnergy = new DateTimeData();
        }

        private void StartEnergyAdding()
        {
            if (_countingDownEnergy != null)
                StopCoroutine(_countingDownEnergy);
            _countingDownEnergy = StartCoroutine(CountingDownAddingEnergy());
        }

        private IEnumerator CountingDownAddingEnergy()
        {
            var elapsed = 0f;
            while (_playerData.playerEnergy < _playerData.playerEnergyMax)
            {
                elapsed += Time.unscaledDeltaTime;
                if (elapsed >= SecondsBetweenAddedEnergy)
                {
                    elapsed = 0;
                    Add(1);
                    UpdateRestoringTimer();
                }
                yield return null;
            }            
        }
        
    }
}