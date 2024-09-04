using UnityEngine;

namespace SleepDev
{
    [System.Serializable]
    public class RectArea
    {
        public float sizeX;
        public float sizeY;
        public Transform center;
        
        public Vector3 UpLeftCorner()
        {
            var pos = center.position;
            pos.x -= sizeX / 2f;
            pos.y += sizeY / 2f;
            return pos;
        }
        
        public Vector3 UpRightCorner()
        {
            var pos = center.position;
            pos.x += sizeX / 2f;
            pos.y += sizeY / 2f;
            return pos;
        }

        public Vector3 DownLeftCorner()
        {
            var pos = center.position;
            pos.x -= sizeX / 2f;
            pos.y -= sizeY / 2f;
            return pos;
        }
    
        public Vector3 DownRightCorner()
        {
            var pos = center.position;
            pos.x += sizeX / 2f;
            pos.y -= sizeY / 2f;
            return pos;
        }

        public void Correct(ref Vector3 localPos)
        {
            if (localPos.y > sizeY / 2f)
                localPos.y = sizeY / 2f;
            if (localPos.y < -sizeY / 2f)
                localPos.y = -sizeY / 2f;
            
            if (localPos.x > sizeX / 2f)
                localPos.x = sizeX / 2f;
            if (localPos.x < -sizeX / 2f)
                localPos.x = -sizeX / 2f;
        }

        public Vector3 GetWorld(Vector3 localPos)
        {
            return center.TransformPoint(localPos);
        }

        public Vector3 GetRandomInside()
        {
            var x = UnityEngine.Random.Range(-sizeX / 2f, sizeX / 2f);
            var y = UnityEngine.Random.Range(-sizeY / 2f, sizeY / 2f);
            var local = new Vector3(x, y, 0);
            return center.TransformPoint(local);
        }
    }
}