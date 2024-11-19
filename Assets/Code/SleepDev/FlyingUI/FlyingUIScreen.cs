using RobotCastle.Core;
using UnityEngine;

namespace SleepDev.FlyingUI
{
    public class FlyingUIScreen : MonoBehaviour
    {

        public static FlyingUIScreen Create()
        {
            var prefab = Resources.Load<FlyingUIScreen>("prefabs/ui/flying_ui");
            var instance = Instantiate(prefab);
            return instance;
        }

        public void On() => gameObject.SetActive(true);

        public void Off() => gameObject.SetActive(false);

        [SerializeField] private Pool _elementsPool;
        [SerializeField] private float _defaultFlyTime = 1f;
        [SerializeField] private float _defaultDelay = .2f;
        [SerializeField] private float _randomRadius = 100;

        private void Awake()
        {
            _elementsPool.Init();
        }

        public void FlyFromTo(Sprite icon, Vector3 fromPos, Vector3 endPos, int count, float time = 0f)
        {
            if (time <= 0f)
                time = _defaultFlyTime;
            for (var i = 0; i < count; i++)
            {
                var it = (FlyingElement)_elementsPool.GetOne();
                it.Pool = _elementsPool;
                it.SetIcon(icon);
                it.ShowText(false);
                var randFromPos = fromPos + (Vector3)UnityEngine.Random.insideUnitCircle * _randomRadius;
                it.FlyFromTo(randFromPos, endPos, time, _defaultDelay);
            }
        }

    }
}