using System;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public abstract class TutorialBase : MonoBehaviour
    {
        [SerializeField] protected FadeInOutAnimator _panelAnimator;
        [SerializeField] protected TutorialTextPrinter _textPrinter;
        [SerializeField] protected TutorialHand _hand;
        [SerializeField] protected RectTransform _parent;
        protected Action _finishedCallback;

        public abstract void Begin(Action finishedCallback);



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