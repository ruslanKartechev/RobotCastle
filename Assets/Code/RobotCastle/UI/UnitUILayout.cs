using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.UI
{
    public class UnitUILayout : MonoBehaviour
    {
        [SerializeField] private List<PieceData> _optionBattle;
        [SerializeField] private List<PieceData> _optionMerge;

        [ContextMenu("Set Battle")]
        public void SetBattle()
        {
            foreach (var data in _optionBattle)
            {
                data.Set();
            }
        }

        [ContextMenu("Set Merge")]
        public void SetMerge()
        {
            foreach (var data in _optionMerge)
            {
                data.Set();
            }
        }


        [System.Serializable]
        private class PieceData
        {
            public RectTransform rect;
            public bool isActive;
            public Vector2 anchorPos;

            public void Set()
            {
                rect.gameObject.SetActive(isActive);
                rect.anchoredPosition = anchorPos;
                #if UNITY_EDITOR
                if (Application.isPlaying)
                    UnityEditor.EditorUtility.SetDirty(rect);
                #endif
            }
        }
    }
}