using UnityEngine;

namespace SleepDev
{
    [System.Serializable]
    public class RectGrid
    {
        public Transform center;
        public float xSize = 1;
        public float ySize = 2;
        public float xSpace = 0.1f;
        public float ySpace = 0.1f;
        public int totalCountX;
        public int totalCountY;
        public Vector3 localCenter;
        private int _frontCenter;
        public Vector3 CenterWorldPoint => center.position;
        
        public void SetCenter(int totalX, int totalY, bool XZ)
        {
            _frontCenter = 1;
            totalCountX = totalX;
            totalCountY = totalY;
            var localY = 0f;
            if (totalCountY % 2 == 0)
                localY -= (totalCountY / 2 - 0.5f) * (ySize + ySpace);
            else
                localY -= (totalCountY / 2) * (ySize + ySpace);   
            
            var localX = 0f;
            if (totalCountX % 2 == 0)
                localX -= (totalCountX / 2 - 0.5f) * (xSize + xSpace);
            else
                localX -= (totalCountX / 2) * (xSize + xSpace);
            
            if(XZ)
                localCenter = new Vector3(localX, 0, localY);
            else
                localCenter = new Vector3(localX, localY, 0);
        }
        
        public void SetCenterFront(int totalX, int totalY, bool XZ)
        {
            _frontCenter = -1;
            totalCountX = totalX;
            totalCountY = totalY;
            var localX = 0f;
            if (totalCountX % 2 == 0)
                localX -= (totalCountX / 2 - 0.5f) * (xSize + xSpace);
            else
                localX -= (totalCountX / 2) * (xSize + xSpace);
            
            if(XZ)
                localCenter = new Vector3(localX, 0, 0);
            else
                localCenter = new Vector3(localX, 0, 0);
        }

        public Vector3 GetWorld(Vector3 local) => center.TransformPoint(local);
        
        public Vector3 GetPositionXZ(int x, int z)
        {
            return localCenter + new Vector3(x * (xSize + xSpace), 
                0
                , _frontCenter * z * (ySize + ySpace));
        }
        
        public Vector3 GetPositionXY(int x, int y)
        {
            return localCenter + new Vector3(x * (xSize + xSpace),  
                _frontCenter * y * (ySize + ySpace), 
                0);
        }
        
    }
}