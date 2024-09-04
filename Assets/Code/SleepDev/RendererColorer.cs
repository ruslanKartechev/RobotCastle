using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    public class RendererColorer : MonoBehaviour
    {
        [SerializeField] private string ColorKey = "_Color";
        [SerializeField] private string _noTextMatColorKey = "_Color";
        
        [SerializeField] private SkinnedMeshRenderer _renderer;
        [SerializeField] private Material _noColorMat;
        [SerializeField] private float _fadeColorStartMultiplier = 1f;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_renderer == null)
            {
                _renderer = MiscUtils.GetFromAllChildren<SkinnedMeshRenderer>(transform,
                    (t) => t != null && t.enabled && t.gameObject.activeInHierarchy).FirstOrDefault();
                EditorUtility.SetDirty(this);
            }
        }
        #endif

        public void SetColor(Color color)
        {
            if (_renderer.sharedMaterials.Length == 1)
            {
                var block = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(block);
                block.SetColor(ColorKey, color);
                _renderer.SetPropertyBlock(block);
                return;
            }
            var count = _renderer.sharedMaterials.Length;
            for (var i = 0; i < count; i++)
                SetColorToIndex(color, i);
        }

        public void FadeToColor(Color color, float time)
        {
            var count = _renderer.sharedMaterials.Length;
            var initialColors = new Color[count];
            var materials = new Material[count];
            for (var i = 0; i < count; i++)
            {
                materials[i] = _renderer.sharedMaterials[i];
                initialColors[i] = materials[i].GetColor(ColorKey) * _fadeColorStartMultiplier;
                materials[i] = _noColorMat;
            }
            _renderer.sharedMaterials = materials;
            StartCoroutine(Fading(initialColors, color, time, count, _noTextMatColorKey));
        }

        private IEnumerator Fading(Color[] from, Color to, float time, int count, string key)
        {
            var blocks = new List<MaterialPropertyBlock>(from.Length);
            for (var i = 0; i < count; i++)
            {
                var block = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(block, i);
                blocks.Add(block);
            }

            var elapsed = 0f;
            while (elapsed <= time)
            {
                var t = elapsed / time;
                for (var i = 0; i < count; i++)
                {
                    var c = Color.Lerp(from[i], to, t);
                    var propBlock = blocks[i];
                    propBlock.SetColor(key, c);   
                    _renderer.SetPropertyBlock(propBlock, i);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        private void SetColorToIndex(Color color, int index)
        {
            var block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(block, index);
            block.SetColor(ColorKey, color);   
            _renderer.SetPropertyBlock(block, index);
        }
    }
}