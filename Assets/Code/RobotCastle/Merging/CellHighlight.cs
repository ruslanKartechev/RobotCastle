using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class CellHighlight : MonoBehaviour, IPoolObject
    {
        public int HighlightType { get; set; }
        
        public void ShowAt(Vector3 worldPos) 
        {
            transform.position = worldPos;
            gameObject.SetActive(true);
        }


        public string PoolId { get; set; }
        public GameObject GetGameObject() => gameObject;
        public void PoolHide() => gameObject.SetActive(false);
        public void PoolShow() => gameObject.SetActive(true);
    }
}