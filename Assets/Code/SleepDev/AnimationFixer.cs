using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SleepDev
{
    public class AnimationFixer : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private string _wordToRemove = "Armature/";
        [SerializeField] private AnimationClip _clip;
        [SerializeField] private List<AnimationClip> _clips;
        [Space(10)]
        [SerializeField] private string _replaceFrom = "rig5/";
        [SerializeField] private string _replaceTo = "rig/";
        [Space(10)] 
        [SerializeField] private Transform _rootBone;

        private bool _logAllLines = false;
        
        private string DataPath
        {
            get
            {
                return Application.dataPath.TrimEnd(new[]{'A', 's', 's','e', 't', 's'});
            }
        }

        [ContextMenu("Fix List of Animations")]
        public void FixList()
        {
            foreach (var clip in _clips)
            {
                FixAnimation(clip, _wordToRemove);
            }
        }

        [ContextMenu("Fix Animation")]
        public void Fix()
        {
            FixAnimation(_clip, _wordToRemove);
        }
        
        public void FixAnimation(AnimationClip clip, string token)
        {
            var filePath = GetPathToAsset(clip);
            CLog.LogWHeader($"AnimationFixer", $"Path: {filePath}", "w");
            if (File.Exists(filePath) == false)
            {
                CLog.LogWHeader($"AnimationFixer", $"Does not exist", "w");
                return;
            }
            var lines = File.ReadAllLines(filePath);
            var correctedLines = new List<string>();
            var correctedLinesCount = 0;
            foreach (var line in lines)
            {
                if (line.Contains(token))
                {
                    CLog.LogWHeader("Corrected", $"{line}", "g", "w");
                    var correctedLine = line.Replace(token, "");
                    correctedLines.Add(correctedLine);
                    correctedLinesCount++;
                }
                else
                {
                    correctedLines.Add(line);
                    if(_logAllLines)
                        CLog.Log($"{line}");
                }
            }
            CLog.LogWHeader("AnimationFixer", $"Fixed lines count: {correctedLinesCount}", "g");
            File.WriteAllLines(filePath, correctedLines);
        }
        
        private string GetPathToAsset(Object obj)
        {
            return DataPath + AssetDatabase.GetAssetPath(obj);
        }

        [ContextMenu("Replace For one clip")]
        public void ReplaceOne()
        {
            ReplaceForClip(_clip, _replaceFrom, _replaceTo);
        }
        
        public void ReplaceForClip(AnimationClip clip, string replaceFrom, string replaceTo)
        {
            var filePath = GetPathToAsset(clip);
            CLog.LogWHeader($"AnimationFixer", $"Path: {filePath}", "w");
            if (File.Exists(filePath) == false)
            {
                CLog.LogWHeader($"AnimationFixer", $"Does not exist", "w");
                return;
            }
            var lines = File.ReadAllLines(filePath);
            var correctedLines = new List<string>();
            var correctedLinesCount = 0;
            foreach (var line in lines)
            {
                if (line.Contains(replaceFrom))
                {
                    CLog.LogWHeader("Corrected", $"{line}", "g", "w");
                    line.Replace(replaceFrom, replaceTo);
                    var correctedLine = line.Replace(replaceFrom, "");
                    correctedLines.Add(correctedLine);
                    correctedLinesCount++;
                }
                else
                {
                    correctedLines.Add(line);
                    if(_logAllLines)
                        CLog.Log($"{line}");
                }
            }
            CLog.LogWHeader("AnimationFixer", $"Fixed lines count: {correctedLinesCount}", "g");
            File.WriteAllLines(filePath, correctedLines);
        }
        
        
        [ContextMenu("Rename All bones")]
        public void ReplaceBoneNames()
        {
            var replaceFrom = _replaceFrom;
            var replaceTo = _replaceTo;
            var bones = MiscUtils.GetFromAllChildren<Transform>(_rootBone);
            var correctedLines = new List<string>();
            var correctedLinesCount = 0;
            foreach (var bone in bones)
            {
                var boneName = bone.gameObject.name;
                if (boneName.Contains(replaceFrom))
                {
                    CLog.LogWHeader("Corrected", $"{bone}", "g", "w");
                    boneName.Replace(replaceFrom, replaceTo);
                    var correctedLine = boneName.Replace(replaceFrom, replaceTo);
                    correctedLines.Add(correctedLine);
                    correctedLinesCount++;
                    bone.gameObject.name = correctedLine;
                    UnityEditor.EditorUtility.SetDirty(bone.gameObject);
                }
                else
                {
                    correctedLines.Add(boneName);
                    if(_logAllLines)
                        CLog.Log($"{bone}");
                }
            }
            CLog.LogWHeader("AnimationFixer", $"Fixed lines count: {correctedLinesCount}", "g");
        }
        #endif

    }
}

// CAMERA FOR FOREST_5
// CAMERA SHAKE FOR KONG-Throwing things