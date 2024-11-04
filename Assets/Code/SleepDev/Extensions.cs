using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public static class Extensions
    {
        public static Vector3 AnglesTo180(this Vector3 angles)
        {
            if (angles.x > 180)
                angles.x -= 360;
            if (angles.y > 180)
                angles.y -= 360;
            if (angles.z > 180)
                angles.z -= 360;
            return angles;
        }

        public static void SetXZPos(this Transform tr, Vector3 position)
        {
            position.y  = tr.position.y;
            tr.position = position;
        }
        
        public static Transform CopyPosRot(this Transform me, Transform other)
        {
            me.SetPositionAndRotation(other.position, other.rotation);
            return me;
        }
        public static Transform CopyPosRotScale(this Transform me, Transform other)
        {
            me.SetPositionAndRotation(other.position, other.rotation);
            me.localScale = other.localScale;
            return me;
        }
        
        public static Transform SetParentAndCopy(this Transform me, Transform parent)
        {
            me.parent = parent;
            me.SetPositionAndRotation(parent.position, parent.rotation);
            return me;
        }
        
        public static Transform SetParentAndCopyAndScale(this Transform me, Transform parent)
        {
            me.parent = parent;
            me.SetPositionAndRotation(parent.position, parent.rotation);
            me.localScale = Vector3.one;
            return me;
        }
        
        public static Transform SetParentAndPos(this Transform me, Transform parent)
        {
            me.parent = parent;
            me.position = parent.position;
            return me;
        }
        public static Transform SetParentAndPos(this Transform me, Transform parent, Vector3 offset)
        {
            me.parent = parent;
            me.localPosition = offset;
            return me;
        }
        
        
        public static T GetOrAdd<T>(GameObject go) where T: Component
        {
            var ss = go.GetComponent<T>();
            if (ss == null)
                ss = go.AddComponent<T>();
            return ss;
        }
        
        public static T Random<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default;
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default;
            var val= list[UnityEngine.Random.Range(0, list.Count)];
            list.Remove(val);
            return val;
        }

        public static float Random(this Vector2 vec)
        {
            return UnityEngine.Random.Range(vec.x, vec.y);
        }

        public static void SetY(this Transform tr, float y)
        {
            var pos = tr.position;
            pos.y = y;
            tr.position = pos;
        }
        
        public static void SetYScale(this Transform tr, float y)
        {
            var scale = tr.localScale;
            scale.y = y;
            tr.localScale = scale;
        }

        public static void SetYPos(this Transform tr, float y)
        {
            var pos = tr.position;
            pos.y = y;
            tr.position = pos;
        }
        
        public static void SetYLocalPos(this Transform tr, float y)
        {
            var pos = tr.localPosition;
            pos.y = y;
            tr.localPosition = pos;
        }

        
        public static float XZDistance2(this Vector3 vec)
        {
            return vec.x * vec.x + vec.z * vec.z;
        }
        
        public static float XZDistance(this Vector3 vec)
        {
            return Mathf.Sqrt(vec.x * vec.x + vec.z * vec.z);
        }
        
        
        public static Vector3 XZPlane(this Vector3 pos)
        {
            pos.y = 0f;
            return pos;
        }
        
        public static Vector2 ToXZPlane(this Vector3 pos)
        {
            return new Vector2(pos.x, pos.z);
        }

        public static Vector3 ToVec3XZ(this Vector2 pos, float y)
        {
            return new Vector3(pos.x, y, pos.y);
        }
        
        public static Vector3 XZPlaneNormalized(this Vector3 pos)
        {
            var vec = new Vector3(pos.x, 0f, pos.z).normalized;
            return vec;
        }
    }
}