using UnityEngine;

namespace RobotCastle.UI
{
    public class TestDescriptionProvider : MonoBehaviour, IItemDescriptionProvider
    {
        [SerializeField] private string _text; 
        [SerializeField] private Sprite _icon;
        
        public string GetIdForUI()
        {
            return "ui_description_test";
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Vector3 WorldPosition => transform.position;

        public string GetText() => _text;
        public Sprite GetIcon() => _icon;
    }
}