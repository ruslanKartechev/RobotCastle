using System;
using RobotCastle.Core;
using TMPro;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class TroopsCountView : MonoBehaviour, ITroopsCountView
    {
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private Animator _animator;

        private void OnEnable()
        {
            ServiceLocator.Bind<ITroopsCountView>(this);
            ServiceLocator.Bind<TroopsCountView>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<ITroopsCountView>();
            ServiceLocator.Unbind<TroopsCountView>();
        }

        public void SetCount(int count, int max)
        {
            _text.text = $"{count}/{max}";
        }

        public void UpdateCount(int count, int max)
        {
            _text.text = $"{count}/{max}";
            _animator.Play("Updated");
        }
    }
}