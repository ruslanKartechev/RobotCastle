﻿using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class SortAllButton : MonoBehaviour
    {
        [SerializeField] private ScaleWorldButton _worldButton;
        [SerializeField] private SoundID _sound;

        private void OnEnable()
        {
            _worldButton.OnClicked += Call;
        }

        private void Call()
        {
            ServiceLocator.Get<MergeManager>().SortAll();
            SoundManager.Inst.Play(_sound);
        }
    }
}