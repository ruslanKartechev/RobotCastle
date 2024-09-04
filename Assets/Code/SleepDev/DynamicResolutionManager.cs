using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class DynamicResolutionManager : MonoBehaviour
    {
        [SerializeField] private float _mediumScale;
        [SerializeField] private float _minScale;
        [Space(10)]
        [SerializeField] private int _mediumAverageFPS = 45;
        [SerializeField] private int _minAverageFPS = 30;
        [Space(10)] 
        [SerializeField] private int _framesSkippedCount = 10;
        [SerializeField] private int _frameRecordingCount = 100;
        private Coroutine _working;
        

        public void Begin()
        {
            _working = StartCoroutine(Working());   
        }

        private IEnumerator Working()
        {
            var framesSkipped = _framesSkippedCount;
            for(var i = 0; i < framesSkipped; i++)
                yield return null;
            var totalCount = _frameRecordingCount;
            var averageFps = 0f;
            for (var i = 0; i < totalCount; i++)
            {
                var fps = 1f / Time.deltaTime;
                averageFps += fps / totalCount;
                yield return null;
            }
            CLog.LogRed($"[DRS] Average fps: {averageFps}");
            if (averageFps <= _minAverageFPS)
            {
                CLog.LogRed($"[DRS] Resolution scale set to: {_minScale}");
                ScalableBufferManager.ResizeBuffers(_minScale, _minScale);
            }
            else if (averageFps <= _mediumAverageFPS)
            {
                CLog.LogYellow($"[DRS] Resolution scale set to: {_mediumScale}");
                ScalableBufferManager.ResizeBuffers(_mediumScale, _mediumScale);
            }
            else if (averageFps <= 55)
            {
                CLog.LogGreen($"[DRS] Resolution scale set to: {1f}");
            }
            else
            {
                CLog.LogGreen($"[DRS] Quality is increased...");
            }
        }
    }
}