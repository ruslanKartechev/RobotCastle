using System;
using System.Collections;
using SleepDev;

namespace RobotCastle.MainMenu
{
    public class TutorialSummonNewHero : TutorialBase
    {
        public override void Begin(Action finishedCallback)
        {
            CLog.LogGreen($"[TutorialBattle] Begin");
            _finishedCallback = finishedCallback;
            StartCoroutine(Working());
        }

        private IEnumerator Working()
        {
            yield return null;
        }
    }
}