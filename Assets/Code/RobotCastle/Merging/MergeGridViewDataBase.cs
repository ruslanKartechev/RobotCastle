using UnityEngine;

namespace RobotCastle.Merging
{
    [CreateAssetMenu(menuName = "SO/MergeGridViewDataBase", fileName = "MergeGridViewDataBase", order = 0)]
    public class MergeGridViewDataBase : ScriptableObject
    {
        public Vector3 draggingOffset;
        public LayerMask cellsMask;
        public Material cellDefaultMaterial; 
        public Material cellHighlightedMaterial;
        [Space(10)]
        public Color mergeItemIdleColor = Color.white;
        public Color mergeItemPickedColor = Color.yellow;
        public float mergeItemIdleScale = 1f;
        public float mergeItemPickedScale = 1.1f;
        public float mergeItemScaleTime = .12f;
        

    }
}