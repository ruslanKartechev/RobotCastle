using UnityEngine;

namespace Bomber
{
    [CreateAssetMenu(menuName = "SO/MapBuilderEditorConfig", fileName = "Map Builder EditorConfig", order = 2)]
    public class MapBuilderEditorConfigSO : ScriptableObject
    {
        public MapBuilderEditorConfig config;
    }
}