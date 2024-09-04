using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev.Ragdoll
{
    public class BonePositioner : MonoBehaviour
    {
        [SerializeField] private Transform bodyParent;
        [SerializeField] private Transform body;
        [SerializeField] private Transform mainBone;
        [SerializeField] private List<Transform> bones;
        private Dictionary<Transform, PositionData> bonePositions;
        private Coroutine _coroutine;
        
        public void Save()
        {
            bonePositions = MakeCurrentPositionsTable();
        }


        public void SetSavedNow()
        {
            body.parent = bodyParent.parent;
            body.position = bodyParent.position = GetBodyPosition();
            foreach (var pair in bonePositions)
            {
                pair.Key.localPosition = pair.Value.localPosition;
                pair.Key.localRotation = pair.Value.localRotation;
            }
            body.parent = bodyParent;
        }

        public Vector3 GetBodyPosition()
        {
            var position = mainBone.position;
            position.y = bodyParent.position.y;
            return position;
        }
        
        public void SetSaved(float duration, Action onEnd)
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Moving(duration, onEnd));
        }

        private IEnumerator Moving(float time, Action onEnd)
        {
            var elapsed = 0f;
            var startTable = MakeCurrentPositionsTable();
            while (elapsed < time)
            {
                var t = elapsed = time;
                foreach (var pair in startTable)
                {
                    pair.Value.Lerp(pair.Key, bonePositions[pair.Key], t);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            foreach (var pair in startTable)
            {
                pair.Value.Lerp(pair.Key, bonePositions[pair.Key], 1);
            }
            onEnd?.Invoke();
        }

        
        
        private Dictionary<Transform, PositionData> MakeCurrentPositionsTable()
        {
            var table = new Dictionary<Transform, PositionData>();
            foreach (var b in bones)
            {
                table.Add(b, new PositionData(b));
            }
            return table;
        }

        
        
        

        private class PositionData
        {
            public PositionData(Transform t)
            {
                this.localPosition = t.localPosition;
                this.localRotation = t.localRotation;
            }
            
            public PositionData(Vector3 localPosition, Quaternion localRotation)
            {
                this.localPosition = localPosition;
                this.localRotation = localRotation;
            }

            public Vector3 localPosition;
            public Quaternion localRotation;


            public void Lerp(Transform target, PositionData end, float t)
            {
                target.localPosition = Vector3.Lerp(localPosition, end.localPosition, t);
                target.localRotation = Quaternion.Lerp(localRotation, end.localRotation, t);
            }
        }
    }
}