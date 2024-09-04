using UnityEngine;

namespace SleepDev
{
    public abstract class OnGizmosDrawer : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] protected bool _doDraw;
        [SerializeField] protected Color _color;

        public bool doDraw
        {
            get => _doDraw;
            set => _doDraw = value;
        }

        public Color color
        {
            get => _color;
            set => _color = value;
        }

        
     #endif   
    }
}