using RobotCastle.Battling;
using UnityEngine;

namespace RobotCastle.UI
{
    public class HeroDescriptionProvider : MonoBehaviour, IItemDescriptionProvider
    {
        [SerializeField] private HeroComponents _heroView;
        
        public string GetIdForUI() => "ui_hero_description";

        public GameObject GetGameObject() => gameObject;
        
        public Vector3 WorldPosition => transform.position;

        public HeroComponents HeroView => _heroView;
    }
}