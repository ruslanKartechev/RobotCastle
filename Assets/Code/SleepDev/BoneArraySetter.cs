using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SleepDev
{
    public class BoneArraySetter : MonoBehaviour
    {
        [SerializeField] private Transform _bonesParent;
        [SerializeField] private List<Transform> _bones;
        [SerializeField] private List<PerRendData> _perRendData;
        [SerializeField] private List<SkinnedMeshRenderer> _copyToRenderers;
        [SerializeField] private Transform _toParent;

        [System.Serializable]
        public class PerRendData
        {
            public List<string> names = new List<string>();
            public string rendererName;
            public SkinnedMeshRenderer fromRenderer;
        }
        
        [ContextMenu("Print From")]
        public void PrintFromBones()
        {
            foreach (var data in _perRendData)
            {
                if(data == null)
                    continue;
                if (data.fromRenderer == null)
                {
                    Debug.Log("Copy from == null");
                    continue;
                }
                Debug.Log("<color=green>***********</color>");
                var it = 1;
                foreach (var bb in data.fromRenderer.bones)
                {
                    Debug.Log($"Bone {it}: {bb.name}");
                    it++;
                }
            }
      
        }
        
        [ContextMenu("Print To")]
        public void PrintToBones()
        {
            foreach (var renderer in _copyToRenderers)
            {
                if (renderer == null)
                {
                    Debug.Log("Copy from == null");
                    continue;
                }
                Debug.Log("<color=green>***********</color>");
                var it = 1;
                foreach (var bb in renderer.bones)
                {
                    Debug.Log($"Bone {it}: {bb.name}");
                    it++;
                }
            }
        }
        
        [ContextMenu("Get names")]
        public void CopyBones()
        {
            if (!CheckNamesMismatch())
            {
                Debug.Log("NO FIT");
                return;
            }
            GetBones();
            FindAndSetBones();
        }

        [ContextMenu("CheckNamesMismatch")]
        public bool CheckNamesMismatch()
        {
            var rendInd = 0;
            foreach (var data in _perRendData)
            {
                if (data.names == null || data.names.Count == 0)
                {
                    Debug.Log("No bone names found");
                    return false;
                }

                if (rendInd >= _copyToRenderers.Count)
                {
                    Debug.Log($"data[] i >= copy_to_renderes count");
                    return false;
                }
                var rend = _copyToRenderers[rendInd];
                var arr1 = rend.bones;
                var arr2 = data.names.ToArray();
                if (arr1.Length != arr2.Length)
                    return false;
                var bonesInd = 0;
                foreach (var toRend in arr1)
                {
                    if (toRend.name != arr2[bonesInd])
                    {
                        Debug.Log($"Bone names mismatch: To: {toRend.name} and From: {arr2[bonesInd]}");
                        return false;
                    }
                    bonesInd++;
                }
                rendInd++;
            }
            return true;
        }
        
     
        [ContextMenu("Get names")]
        public void GetBonesNames()
        {
            foreach (var data in _perRendData)
            {
                var from = data.fromRenderer;
                if (from == null)
                {
                    Debug.Log("Copy from == null");
                    return;
                }
                data.rendererName = data.fromRenderer.name;
                var bones = from.bones;
                data.names?.Clear();
                data.names = new List<string>();
                foreach (var bb in bones)
                {
                    if (bb == null)
                        continue;
                    data.names.Add(bb.name);
                }
            }
        }        
        
        [ContextMenu("Copy bones")]
        public void FindAndSetBones()
        {
            for (var i = 0; i < _perRendData.Count; i++)
            {
                var from = _perRendData[i].fromRenderer;
                var names = _perRendData[i].names;
                if (i >= _copyToRenderers.Count)
                {
                    Debug.Log($"To_meshes count < From_meshes count");
                    return;
                }
                if (names.Count == 0)
                {
                    Debug.Log("No bone names assigned");
                    continue;
                }
                var array = new List<Transform>();
                foreach (var name in names)
                {
                    var found = true;
                    var bone = _bones.Find(t => t.name == name);
                    if (bone == null)
                        found = false;
                    if (!found)
                    {
                        Debug.Log($"Cannot find {name}");
                        continue;
                    }
                    array.Add(bone);
                }
                var copyTo = _copyToRenderers[i];
                copyTo.bones = array.ToArray();
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(copyTo);
#endif
            }
        }   
        
        
        [ContextMenu("Get bones")]
        public void GetBones()
        {
            if (_bonesParent == null)
                _bonesParent = transform;
            _bones = MiscUtils.GetFromAllChildren<Transform>(_bonesParent);
            Debug.Log($"[GetBones] got {_bones.Count} bones");
            foreach (var bb in _bones)
            {
                // Debug.Log(bb.name);
            }
        }

        [ContextMenu("GetCopyToByNames")]
        public void GetCopyToByNames()
        {
            _copyToRenderers?.Clear();
            _copyToRenderers = new List<SkinnedMeshRenderer>();
            foreach (var data in _perRendData)
            {
                var name = data.rendererName;
                var parent = transform.parent;
                var renderer = MiscUtils.GetFromAllChildren<SkinnedMeshRenderer>(parent,
                    (t) => { return t != null &&  t.gameObject.name == name; }).FirstOrDefault();
                if (renderer != null)
                {
                    _copyToRenderers.Add(renderer);
                }
                else
                {
                    Debug.Log($"Renderer with name: {name} not found in hierarchy");
                    continue;
                }
            }
        }

        [ContextMenu("GetCopyToByRefs")]
        public void GetCopyToByRefs()
        {
            var count = _toParent.childCount;
            _copyToRenderers?.Clear();
            _copyToRenderers = new List<SkinnedMeshRenderer>();
            for (var i = 0; i < count; i++)
            {
                if (_toParent.GetChild(i).TryGetComponent<SkinnedMeshRenderer>(out var rend))
                {
                    _copyToRenderers.Add(rend);
                }
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}