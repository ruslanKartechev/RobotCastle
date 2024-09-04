#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev.Chunks
{
    [CustomEditor(typeof(ChunkManager))]
    public class ChunkManagerEditor : Editor
    {
 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as ChunkManager;
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("Activate", EU.Aqua))
                me.Activate();
            if (EU.BtnMid2("Stop", EU.Aqua))
                me.Deactivate();
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("ShowAll", EU.Aqua))
                me.ActivateAll();
            if (EU.BtnMid2("HideAll", EU.Aqua))
                me.DeactivateAll();
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("InFrustrum", EU.Aqua))
                me.ActivateInFrustum();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("Build", EU.Lavender))
                me.E_BuildChunks();
            if (EU.BtnMid2("Delete", EU.Lavender))
                me.E_DeleteChunks();
            GUILayout.EndHorizontal();
            if (EU.BtnMid2("Fetch Chunks", EU.Lavender))
                me.E_FetchChunksFromBuilder();
        }
    }
}
#endif