﻿using System;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.MainMenu
{
    public class EnergyOffer
    {
        public void MakeOffer(Action callback)
        {   
            _callback = callback;
            var ui = ServiceLocator.Get<IUIManager>().Show<EnergyOfferUI>(UIConstants.UIEnergyOffer, () => {});
                ui.Show(PlayerEnergyManager.EnergyGivenPerOffer, AcceptCallback);
        }
        

        private const string PlacementName = "more_energy";
        private Action _callback;

        private void AcceptCallback(bool didAccept)
        {
            if (didAccept)
            {
                var (res, msg) = AdsPlayer.Instance.PlayReward(OnAdPlayed, PlacementName);
                CLog.Log($"[{nameof(EnergyOffer)}] play rewarded: {res}. {msg}");

            }
            else
                _callback?.Invoke();
        }

        private void OnAdPlayed(bool didPlay)
        {
            if (didPlay)
            {
                CLog.Log($"[{nameof(EnergyOffer)}] On Ad Played, giving reward");
                ServiceLocator.Get<PlayerEnergyManager>().Add(PlayerEnergyManager.EnergyGivenPerOffer);
            }
            else
            {
                CLog.Log($"[{nameof(EnergyOffer)}] On Ad Played Failed, NO reward");
            }
            _callback?.Invoke();
        }
    }
}