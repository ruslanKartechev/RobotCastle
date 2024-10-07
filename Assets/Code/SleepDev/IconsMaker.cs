using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
#if UNITY_EDITOR
    public class IconsMaker : MonoBehaviour
    {
        [SerializeField] private bool _overrideIfFileExists;
        [SerializeField] private float _delayBeforeShot;
        [SerializeField] private float _delayAfterShot;
        [SerializeField] private string _namePrefix;
        [SerializeField] private string _absoluteSavePath;
        [SerializeField] private string _format = ".png";
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private Vector2Int _readTextureSize;
        [SerializeField] private Vector2Int _readTexturePos;
        [SerializeField] private List<GameObject> _objects;
        [Space(15)] 
        [SerializeField] private GameObject _current;
        private Coroutine _working;

        public void Next()
        {
            if (_objects.Count == 0)
                return;
            if (_current == null)
            {
                _current = _objects[0];
                _current.SetActive(true);
                return;
            }
            _current.SetActive(false);
            var index = _objects.IndexOf(_current);
            index++;
            if (index >= _objects.Count)
                index = 0;
            _objects[index].SetActive(true);
            _current = _objects[index];
        }

        public void Prev()
        {
            if (_objects.Count == 0)
                return;
            if (_current == null)
            {
                _current = _objects[0];
                _current.SetActive(true);
                return;
            }
            _current.SetActive(false);
            var index = _objects.IndexOf(_current);
            index--;
            if (index < 0)
                index = _objects.Count - 1;
            _objects[index].SetActive(true);
            _current = _objects[index];
        }
        

        public void TakeCurrent()
        {
            if (_current == null)
            {
                var active = new List<GameObject>(3);
                foreach (var go in _objects)
                    active.Add(go);
                if (active.Count == 0 || active.Count > 1)
                {
                    CLog.LogRed("Cannot pick active from list.");
                    return;
                }
                _current = active[0];
            }
            TakeFor(_current);
        }
        
        public void TakeAll()
        {
            if (_renderTexture == null)
            {
                CLog.LogRed($"Render Texture is null");
                return;
            }
            if(_working != null)
                StopCoroutine(_working);
            _working = StartCoroutine(Taking());
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private IEnumerator Taking()
        {
            var i = 0;
            var count = _objects.Count;
            foreach (var go in _objects)
                go.SetActive(false);
            if (gameObject.TryGetComponent<Camera>(out var cam))
            {
                cam.targetTexture = _renderTexture;
            }
            while (i < count)
            {
                var go = _objects[i];
                go.SetActive(true);
                yield return new WaitForSeconds(_delayBeforeShot);
                TakeFor(go);
                yield return new WaitForSeconds(_delayAfterShot);
                go.SetActive(false);
                i++;
                yield return null;
            }
            AssetDatabase.Refresh();
        }

        private void TakeFor(GameObject go)
        {
            go.SetActive(true);
            Graphics.SetRenderTarget(_renderTexture);
            var texture = new Texture2D(_readTextureSize.x, _readTextureSize.y, TextureFormat.ARGB32, false);
            var rect = new Rect(_readTexturePos, _readTextureSize);
            texture.ReadPixels(rect, _readTexturePos.x, _readTexturePos.y);
            var bytes = texture.EncodeToPNG();
            var filePath =  $"{_absoluteSavePath}/{_namePrefix}{go.name}{_format}";
            if (_overrideIfFileExists)
            {
                if(File.Exists(filePath))
                    File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, bytes);
            CLog.LogGreen($"Took shot: {_namePrefix}{go.name}{_format}");
        }
        
    }
#endif

    #if UNITY_EDITOR
    [CustomEditor(typeof(IconsMaker))]
    public class IconsMakerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as IconsMaker;
            EU.Space();
            GUILayout.BeginHorizontal();
            if(EU.BtnMid2("Take", EU.ForestGreen))
                me.TakeAll();
            if(EU.BtnMid2("Stop", EU.Plum))
                me.Stop();
            GUILayout.EndHorizontal();
            EU.Space();
            EU.Space();

            GUILayout.BeginHorizontal();
            if(EU.BtnSmallSquare("<<", EU.Gold))
                me.Prev();
            if(EU.BtnSmallSquare(">>", EU.Gold))
                me.Next();
            GUILayout.EndHorizontal();
            EU.Space();

            if(EU.BtnMidWide2("Take Current", EU.Lavender))
                me.TakeCurrent();
            
        }
    }
    #endif
}