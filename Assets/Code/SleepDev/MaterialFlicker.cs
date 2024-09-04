using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(MaterialFlicker))]
    public class MaterialFlickerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as MaterialFlicker;
            GUILayout.Space((10));
            if (EU.BtnMid2("Flick", EU.Fuchsia))
            {
                me.Flick();
            }
        }
    }
    #endif
    
    
    public class MaterialFlicker : MonoBehaviour
    {
        private const string ColorKey = "_Color"; // URP key ?/
        
        [SerializeField] private int _defaultFlickCount = 2;
        [SerializeField] private float _defaultFlickTime = .08f;
        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _flickColor = Color.red;
        private Coroutine _working;
        
        public int defaultFlickCount
        {
            get => _defaultFlickCount;
            set => _defaultFlickCount = value;
        }

        public float defaultFlickTime
        {
            get => _defaultFlickTime;
            set => _defaultFlickTime = value;
        }

        public List<Renderer> renderers
        {
            get => _renderers;
            set => _renderers = value;
        }

        public Color normalColor
        {
            get => _normalColor;
            set
            {
                _normalColor = value;
#if UNITY_EDITOR
                if(Application.isPlaying == false)
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        public Color flickColor
        {
            get => _flickColor;
            set
            {
                _flickColor = value;
#if UNITY_EDITOR
                if(Application.isPlaying == false)
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

     

        public float FlickTime
        {
            get => _defaultFlickTime;
            set => _defaultFlickTime = value;
        }
        
        public void Flick(int count)
        {
            StopFlicking();
            _working = StartCoroutine(Flicking(count, _defaultFlickTime));
        }

        public void Flick()
        {
            StopFlicking();
            _working = StartCoroutine(Flicking(_defaultFlickCount, _defaultFlickTime));
        }

        public void SetNormalColor()
        {
            for (var ri = 0; ri < _renderers.Count; ri++)
            {
                var renderer = _renderers[ri];
                for (var i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    var block = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(block,i);
                    block.SetColor(ColorKey, _normalColor);
                    renderer.SetPropertyBlock(block, i);
                }
            }
     
        }

        public void StopFlicking()
        {
            if (_working != null)
                StopCoroutine(_working);
            SetNormalColor();
        }
        
        private IEnumerator Flicking(int times, float time)
        {
            var blocks = new List<MaterialPropertyBlock>(10);
            for (var ri = 0; ri < _renderers.Count; ri++)
            {
                var renderer = _renderers[ri];
                for (var i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    var block = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(block,i);
                    blocks.Add(block);
                    renderer.SetPropertyBlock(block, i);
                }
            }
            for (var fi = 0; fi < times; fi++)
            {
                SetColor(_flickColor);
                SetBlocks();
                yield return new WaitForSeconds(time);
                SetColor(_normalColor);
                SetBlocks();
                yield return new WaitForSeconds(time);
            }

            void SetBlocks()
            {
                for (var ri = 0; ri < _renderers.Count; ri++)
                {
                    var renderer = _renderers[ri];
                    for (var i = 0; i < renderer.sharedMaterials.Length; i++)
                        renderer.SetPropertyBlock(blocks[i], i);
                }
            }
            
            void SetColor(Color color)
            {
                foreach (var block in blocks)
                    block.SetColor(ColorKey, color);
            }
        }
    }
}