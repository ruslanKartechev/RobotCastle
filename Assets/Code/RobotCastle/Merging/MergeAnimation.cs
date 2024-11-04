using System;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeAnimation : MonoBehaviour
    {
        public void PlayUpgradeOnly(IItemView item)
        {
            gameObject.SetActive(true);
            transform.position = item.Transform.position;
            _particle.Play();
            item.UpdateViewToData();
            item.OnMerged();
            SoundManager.Inst.Play(_sound);
        }
        
        public void Play(IItemView standing, IItemView moving, IGridView gridView, Action endCallback)
        {
            _moving = moving;
            _standing = standing;
            _callback = endCallback;
            _gridView = gridView;
            transform.position = standing.Transform.position;
            _originalParent = moving.Transform.parent;
            standing.Transform.parent = _pointStanding;
            moving.Transform.parent = _pointMoving;
            
            moving.Transform.localPosition = Vector3.zero;
            standing.Transform.localPosition = Vector3.zero;
            _animator.enabled = true;
            _animator.Play("Merge", 0, 0);
        }

        public void AE_Merged()
        {
            _particle.Play();
            MergeFunctions.ClearCellAndHideItem(_gridView, _moving);
            _standing.UpdateViewToData();
            _standing.OnMerged();
            SoundManager.Inst.Play(_sound);
        }

        public void AE_End()
        {
            _moving.Transform.parent = _originalParent;
            _standing.Transform.parent = _originalParent;
            _callback?.Invoke();
        }

        private IItemView _moving;
        private IItemView _standing;
        private IGridView _gridView;
        private Action _callback;
        private Transform _originalParent;
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private Transform _pointStanding;
        [SerializeField] private Transform _pointMoving;
        [SerializeField] private SoundID _sound;

        private void Start()
        {
            ServiceLocator.Bind<MergeAnimation>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unbind<MergeAnimation>();
        }


    }
}