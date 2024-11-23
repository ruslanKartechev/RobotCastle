using System;
using System.Collections;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public abstract class TutorialBase : MonoBehaviour
    {
        
        public abstract void Begin(Action finishedCallback);
        
        public abstract  string Id { get; }

        [SerializeField] protected FadeInOutAnimator _panelAnimator;
        [SerializeField] protected TutorialTextPrinter _textPrinter;
        [SerializeField] protected TutorialHand _hand;
        [SerializeField] protected RectTransform _parent;
        protected Action _finishedCallback;
        protected bool _isWaiting;

        protected IEnumerator WaitForBtn(MyButton btn)
        {
            btn.AddMainCallback(StopWaiting);
            _isWaiting = true;
            while(_isWaiting)
                yield return null;
            btn.RemoveMainCallback(StopWaiting);
        }
        
        protected IEnumerator WaitForBtn(UnityEngine.UI.Button btn)
        {
            btn.onClick.AddListener(StopWaiting);
            _isWaiting = true;
            while(_isWaiting)
                yield return null;
            btn.onClick.RemoveListener(StopWaiting);
        }
        
        protected void StopWaiting() => _isWaiting = false;


        public void SendEventCompleted()
        {
            SleepDev.Analytics.OnTutorialCompleted(Id);
        }
        
        protected class SubParent
        {
            private Transform _child;
            private Transform _parent;            
            private Transform _prevParent;            
            
            public void Parent(Transform child, Transform parent)
            {
                _child = child;
                _parent = parent;
                _prevParent = _child.parent;
                _child.SetParent(_parent);
            }

            public void Return()
            {
                _child.SetParent(_prevParent);
            }
            
        }
    }
}