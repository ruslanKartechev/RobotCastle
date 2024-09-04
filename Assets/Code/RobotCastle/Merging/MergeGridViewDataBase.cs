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
        
    }
}