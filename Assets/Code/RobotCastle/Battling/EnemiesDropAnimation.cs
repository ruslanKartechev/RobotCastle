using System.Collections.Generic;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class EnemiesDropAnimation : MonoBehaviour
    {
        [SerializeField] private List<Transform> _points;
        [SerializeField] private Animator _animator;

        public void AE_Hidden()
        {
            gameObject.SetActive(false);
        }
        
        public void Show(Vector3 rootPos, List<IItemView> views)
        {
            gameObject.SetActive(true);
            transform.position = rootPos;
            var count = views.Count;
            for (var i = 0; i < count; i++)
            {
                var viewTr = Object.Instantiate(views[i].Transform.gameObject).transform;
                _points[i].gameObject.SetActive(true);
                viewTr.position = _points[i].position;
                viewTr.parent = _points[i];
                viewTr.gameObject.SetActive(true);
            }
            for (var i = count; i < _points.Count; i++)
            {
                _points[i].gameObject.SetActive(false);
            }
            _animator.Play("Show", 0, 0);
        }
    }
}