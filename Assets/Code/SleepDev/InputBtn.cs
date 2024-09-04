using UnityEngine;

namespace SleepDev
{
    public class InputBtn : MonoBehaviour
    {
        public const string Path = "prefabs/ui/input_btn";
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ProperButton _btnInput;

        private static InputBtn _inst;

        public static InputBtn Inst => _inst;
        
        public ProperButton Btn => _btnInput;
        
        public static InputBtn Create()
        {
            if (_inst != null)
            {
                CLog.Log("[InputBtn] Already created");
                return _inst;
            }
            var prefab = Resources.Load<InputBtn>(path:Path);
            if (prefab == null)
            {
                CLog.LogError($"[InputBtn] No prefab at {Path}");
                return null;
            }
            var inst = Instantiate(prefab);
            DontDestroyOnLoad(inst);
            _inst = inst;
            return inst;
        }

        public void SetSorting(int order)
        {
            _canvas.sortingOrder = order;
        }

        public void On() => gameObject.SetActive(true);
        
        public void Off() => gameObject.SetActive(false);


    }
}