using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SleepDev
{
    public static class MiscUtils
    {
        public delegate bool Condition<T>(T target);

        public static float RandomInVec(this Vector2 limits)
        {
            return UnityEngine.Random.Range(limits.x, limits.y);
        }
        
        public static int GetRandom(int min, int max, int except)
        {
            for(int i = 0; i < 10; i++)
            {
                int value = Random.Range(min, max);
                if (value != except)
                    return value;
            }
            return Random.Range(min, max);
        }

        public static Transform GetClosestPoint(this List<Transform> points, Vector3 center)
        {
            var result = points[0];
            var shortest = float.MaxValue;
            foreach (var p in points)
            {
                var d2 = (p.position - center).sqrMagnitude;
                if (d2 < shortest)
                {
                    shortest = d2;
                    result = p;
                }
            }
            return result;
        }

        
        public static string ShortConvert(this int value)
        {
            return FormatNumber(value);
        }
        
        
        private static string FormatNumber(int num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return (num / 1000000).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return (num / 1000).ToString("0.#k");
            }
            if (num >= 10000)
            {
                return (num / 1000).ToString("0.##k");
            }

            return num.ToString("#,0");
        }

        public static T GetRandom<T>(this List<T> list, int prevIndex = -1)
        {
            if (list == null || list.Count == 0)
                return default(T);

            if (prevIndex == -1 || list.Count == 1)
                return list[Random.Range(0, list.Count)];

            int maxIterations = 10;
            int iteration = 0;
            int index = 0;
            while (iteration < maxIterations)
            {
                index = Random.Range(0, list.Count);
                if (index == prevIndex)
                {
                    iteration++;
                    continue;
                }
                return list[index];
            }
            return list[index];
        }
        public static void Shuffle<T>(this System.Random random, IList<T> array)
        {
            int n = array.Count;
            while (n > 1)
            {
                int k = random.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static Coroutine Delayed(this MonoBehaviour behaviour, System.Action action, float delay)
        {
            return behaviour.StartCoroutine(behaviour.DelayedCour(action, delay));
        }
        private static IEnumerator DelayedCour(this MonoBehaviour behaviour, System.Action action, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            action?.Invoke();

            yield break;
        }
        public static Coroutine Delayed(this MonoBehaviour behaviour, System.Action action, YieldInstruction instruction)
        {
            return behaviour.StartCoroutine(behaviour.DelayedCour(action, instruction));
        }
        private static IEnumerator DelayedCour(this MonoBehaviour behaviour, System.Action action, YieldInstruction instruction)
        {
            yield return instruction;

            action?.Invoke();

            yield break;
        }
        
        public static bool IsChildOf(Transform obj, Transform parent)
        {
            var pp = obj.parent;
            while (pp != null)
            {
                if (pp == parent)
                    return true;
                pp = pp.parent;
            }
            return false;
        }
        
        public static AT GetOrAdd<AT, AB>(this GameObject root) where AT : Component where AB : AT
        {
            var t = root.GetComponent<AT>();
            if (t == null)
                t = root.AddComponent<AB>();
            return t;
        }
        
        public static T GetOrAdd<T>(this GameObject root) where T: Component
        {
            var t = root.GetComponent<T>();
            if (t == null)
                t = root.AddComponent<T>();
            return t;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static List<T> GetFromAllChildren<T>(Transform parent, Condition<T> condition = null) where T : class
        {
            var list = new List<T>();
            var t = parent.gameObject.GetComponent<T>();
            if (t != null)
            {
                if( (condition != null && condition(t) ) || condition == null)
                    list.Add(t);
            }
            GetFromChildrenAndAdd(list, parent.transform, condition);
            
            return list;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void GetFromChildrenAndAdd<T>(ICollection<T> list, Transform parent, Condition<T> condition = null)
        {
            if (parent.childCount == 0)
                return;
            for (var i = 0; i < parent.childCount; i++)
            {
                var target = parent.GetChild(i).GetComponent<T>();
                if (target != null )
                {
                    if( (condition != null && condition(target) ) || condition == null)
                        list.Add(target);
                }
                GetFromChildrenAndAdd<T>(list, parent.GetChild(i), condition);
            }
        }
        
        public static GameObject FindInChildren(Transform parent, Condition<GameObject> condition)
        {
            if (parent.childCount == 0)
                return null;
            GameObject result = null;
            for (var i = 0; i < parent.childCount; i++)
            {
                var go = parent.GetChild(i).gameObject;
                if (condition(go))
                    return go;
                result = FindInChildren(go.transform, condition);
                if (result != null)
                    return result;
            }
            return result;
        }
        
        
        public static List<Transform> GetAllChildrenAndRename(Transform parent, string name)
        {
            var list = new List<Transform>();
            var count = parent.childCount;
            for (var i = 0; i < count; i++)
            {
                var tr = parent.GetChild(i);
                tr.gameObject.name = $"{name}_{i}";
                list.Add(tr);
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(tr.gameObject);
                #endif
            }
            return list;
        }

        public static void ClampLocalRotation(this Transform transform, Vector3 bounds)
        {
            transform.localRotation = ClampQuaternion(transform.localRotation, bounds);
        }        
        
        public static Quaternion ClampQuaternion(Quaternion q, Vector3 bounds)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;
            var angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
 
            var angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
            angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);
 
            var angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
            angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);
            return q;
        }
        
        
        
        
        
        
        public static T FindClosestTo<T>(ICollection<T> collection, Vector3 center) where T : MonoBehaviour
        {
            T result = null;
            var minD2 = float.MaxValue;
            foreach (var item in collection)
            {
                var d2 = (item.transform.position - center).sqrMagnitude;
                if (d2 <= minD2)
                {
                    minD2 = d2;
                    result = item;
                }
            }
            return result;
        }
        
        public static Transform FindClosestTo(ICollection<Transform> collection, Vector3 center)
        {
            Transform result = null;
            var minD2 = float.MaxValue;
            foreach (var item in collection)
            {
                var d2 = (item.transform.position - center).sqrMagnitude;
                if (d2 <= minD2)
                {
                    minD2 = d2;
                    result = item;
                }
            }
            return result;
        }
        
        public static void Destroy(GameObject go)
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
                 UnityEngine.Object.Destroy(go);
            else
                UnityEngine.Object.DestroyImmediate(go);
#else
                UnityEngine.Object.Destroy(go);
#endif
            
        }
        
        
        
        
        
        
        public static T[] ExtendAndCopy<T>(T[] original, int addedLength)
        {
            T[] tempArray = new T[original.Length + addedLength];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }
            
            return tempArray;
        }
        public static T[] CopyFromArray<T>(this T[] original, T[] from)
        {
            T[] tempArray = new T[original.Length + from.Length];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }

            foreach (var item in from)
            {
                tempArray[i] = item;
                i++;
            }
            return tempArray;
        }

        public static T[] AddToArray<T>(this T[] original, T nextItem)
        {
            T[] tempArray = new T[original.Length + 1];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }
            tempArray[i] = nextItem;
            return tempArray;

        }

        public static GameObject Spawn(GameObject prefab, Transform transform)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
            }
#endif
            var inst = Object.Instantiate(prefab, transform);
            return inst;
        }
        
        public static T Spawn<T>(T prefab, Transform transform) where T: MonoBehaviour
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform) as T;
            }
#endif
            var inst = Object.Instantiate(prefab, transform);
            return inst;
        }

        public static Vector2 GetAnchoredPositionOnCameraCanvas(Vector2 screenPosition, Camera camera, Rect parentRect)
        {
            screenPosition.x *= parentRect.width / camera.pixelWidth;
            screenPosition.y *= parentRect.height / camera.pixelHeight;
            return screenPosition - parentRect.size / 2f;
        }
        
        public static Vector2 GetAnchoredPositionOnCameraCanvas(Vector2 screenPosition, Camera camera, Canvas canvas)
        {
            screenPosition.x *= canvas.pixelRect.width / camera.pixelWidth;
            screenPosition.y *= canvas.pixelRect.height / camera.pixelHeight;
            return screenPosition - canvas.pixelRect.size / 2f;
        }
        
        public static Vector2 GetAnchoredPositionOnCameraCanvas(Vector3 worldPoint, Camera camera, Canvas canvas)
        {
            Vector2 screenPosition = camera.WorldToScreenPoint(worldPoint);
            screenPosition.x *= canvas.pixelRect.width / camera.pixelWidth;
            screenPosition.y *= canvas.pixelRect.height / camera.pixelHeight;
            return screenPosition - canvas.pixelRect.size / 2f;
        }
        
        public static Vector2 GetAnchoredPositionOnCameraCanvas(Transform worldPoint, Camera camera, Canvas canvas)
        {
            Vector2 screenPosition = camera.WorldToScreenPoint(worldPoint.position);
            screenPosition.x *= canvas.pixelRect.width / camera.pixelWidth;
            screenPosition.y *= canvas.pixelRect.height / camera.pixelHeight;
            return screenPosition - canvas.pixelRect.size / 2f;
        }

        public static List<T> RemoveNulls<T>(this List<T> list) where T : class
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                    list.RemoveAt(i); 
            }
            return list;
        }

        public static Image SetAlpha(this Image image, float alpha)
        {
            var col = image.color;
            col.a = alpha;
            image.color = col;
            return image;
        }
    }
}