﻿using System.Collections;
using System.Collections.Generic;
using Bomber;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class CrescentSlashView : MonoBehaviour
    {
        private const float cellRadius = 1f;
        
        [SerializeField] private List<ParticleSystem> _particles;
        private IMap _map;
        private float _speed;
        
        
        public void Launch(int level, CellsMask cellMask, float speed, 
            List<IHeroController> enemies, IDamageSource damageSource)
        {
            _map = ServiceLocator.Get<IMap>();
            _speed = speed;
            gameObject.SetActive(true);
            for (var i = 0; i < _particles.Count; i++)
            {
                var p = _particles[i];
                p.gameObject.SetActive(i == level);
                if (i == level)
                {
                    p.gameObject.SetActive(true);
                    p.Play();
                }
                else
                {
                    p.gameObject.SetActive(false);
                }
            }
            StartCoroutine(Running(cellMask, enemies, damageSource));
        }

        private IEnumerator Running(CellsMask cellMask, List<IHeroController> enemies, IDamageSource damageSource)
        {
            var frw = transform.forward;
            var startCell = _map.GetCellPositionFromWorld(transform.position);
            var cell = startCell;
            var startPos = _map.GetWorldFromCell(cell);
            var cellDir = new Vector2Int(Mathf.RoundToInt(frw.x), Mathf.RoundToInt(frw.z));
            frw = new Vector3(cellDir.x, 0,cellDir.y);
            var cells = new List<Vector2Int>(5);
            var distances = new List<float>(5);
            var alreadyDamaged = new List<IDamageReceiver>(10);
            var damageArgs = damageSource.CalculateSpellDamage();

            for (var i = 0; i < SpellCrescentSlash.MaxDistance; i++)
            {
                cell += cellDir;
                startPos += frw;
                if (_map.IsOutOfBounce(cell))
                    break;
                cells.Add(cell);
                distances.Add(i + 1);
            }
            var pos = transform.position;
            var doMove = true;
            var totalDistance = 0f;
            var cellInd = 0;
            DamageFromCell(startCell);
            while (doMove)
            {
                var dist = Time.deltaTime * _speed;
                totalDistance += dist;
                pos += frw * dist;
                // transform.position = pos;
                if (distances[cellInd] <= totalDistance)
                {
                    DamageFromCell(cells[cellInd]);
                    cellInd++;
                    if (cellInd >= distances.Count)
                        doMove = false;
                }
                yield return null;
            }
            gameObject.SetActive(false);

            void DamageFromCell(Vector2Int center)
            {
                var cellsAround = cellMask.GetCellsAround(center, _map);
                foreach (var c in cellsAround)
                {
                    var worldPoint = _map.GetWorldFromCell(c);
                    for (var i = enemies.Count - 1; i >= 0; i--)
                    {
                        var en = enemies[i];
                        if (en.IsDead || alreadyDamaged.Contains(en.Components.damageReceiver))
                            continue;
                        var d2 = (en.Components.transform.position - worldPoint).sqrMagnitude;
                        if (d2 < cellRadius)
                        {
                            damageSource.Damage(en.Components.damageReceiver, damageArgs);
                            alreadyDamaged.Add(en.Components.damageReceiver);
                        }
                    }
                }
            }
            
        }
        
    }
}