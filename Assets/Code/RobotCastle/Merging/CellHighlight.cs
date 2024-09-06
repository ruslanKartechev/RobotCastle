using UnityEngine;

namespace RobotCastle.Merging
{
    public class CellHighlight : MonoBehaviour
    {
        public int HighlightType { get; set; }
        
        public void ShowAt(Vector3 worldPos) 
        {
            transform.position = worldPos;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}