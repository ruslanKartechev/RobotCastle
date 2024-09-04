using UnityEngine;

namespace SleepDev
{
    [System.Serializable]
    public class TransformData
    {
        public Vector3 pos;
        public Vector3 eulers;
        public Vector3 scale;

        public void Copy(Transform tr)
        {
            pos = tr.position;
            eulers = tr.eulerAngles;
            scale = tr.localScale;
        }

        public void Set(Transform tr)
        {
            tr.position = pos;
            tr.eulerAngles = eulers;
            tr.localScale = scale;
        }

        public void SetLocal(Transform tr)
        {
            tr.localPosition = pos;
            tr.localEulerAngles = eulers;
            tr.localScale = scale;
        }
    }
}