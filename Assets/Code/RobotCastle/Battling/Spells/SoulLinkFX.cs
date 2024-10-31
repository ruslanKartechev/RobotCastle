using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SoulLinkFX : MonoBehaviour
    {
        [SerializeField] private List<Segment> _segments;
        private Coroutine _working;

        public void Show(List<IHeroController> enemies)
        {
            gameObject.SetActive(true);
            foreach (var seg in _segments)
                seg.Hide();
            switch (enemies.Count)
            {
                case 2:
                    _segments[0].Show(enemies[0], enemies[1]);
                    break;
                case 3:
                    _segments[0].Show(enemies[0], enemies[1]);
                    _segments[1].Show(enemies[1], enemies[2]);
                    _segments[2].Show(enemies[2], enemies[0]);
                    break;
                default: return;
            }            
            if (_working != null)
                StopCoroutine(_working);
            
            _working = StartCoroutine(Working());
        }

        public void Remove(IHeroController enemy)
        {
            for (var i = _segments.Count-1; i >= 0; i--)
            {
                var seg = _segments[i];
                if (seg.IsActive)
                {
                    var didRemove = seg.HideIfBound(enemy);
                }
            }
        }

        public void HideAll()
        {
            if (_working != null)
                StopCoroutine(_working);
            for (var i = _segments.Count-1; i >= 0; i--)
            {
                var seg = _segments[i];
                if (seg.IsActive)
                    seg.Hide();
            }
            gameObject.SetActive(false);
        }

        private IEnumerator Working()
        {
            while (true)
            {
                for (var i = _segments.Count-1; i >= 0; i--)
                {
                    var seg = _segments[i];
                    if (seg.IsActive)
                    {
                        seg.Recalculate();
                    }
                }
                yield return null;
            }
        }

        [System.Serializable]
        private class Segment
        {
            private const float UpOffset = .5f;
            
            [SerializeField] private LineRenderer _renderer;
            private IHeroController _en1;
            private IHeroController _en2;
            public bool IsActive { get; private set; }
            
            public void Show(IHeroController en1, IHeroController en2)
            {
                IsActive = true;
                _en1 = en1;
                _en2 = en2;
                Recalculate();
                _renderer.useWorldSpace = true;
                _renderer.enabled = true;
                _renderer.gameObject.SetActive(true);
            }

            public bool HideIfBound(IHeroController deadHero)
            {
                if (_en1 == deadHero || _en2 == deadHero)
                {
                    Hide();
                    return true;
                }
                return false;
            }
            
            public void Hide()
            {
                IsActive = false;
                _renderer.gameObject.SetActive(false);
            }

            public void Recalculate()
            {
                _renderer.SetPosition(0, _en1.Components.transform.position + Vector3.up * UpOffset);
                _renderer.SetPosition(1, _en2.Components.transform.position + Vector3.up * UpOffset);
            }
        }
    }
}