using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SleepDev
{
    public class DynamicResolutionTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _screenText;
        [SerializeField] private Button _incrementBtn;
        [SerializeField] private Button _decrementBtn;
        
        FrameTiming[] frameTimings = new FrameTiming[3];

        public float maxResolutionWidthScale = 1.0f;
        public float maxResolutionHeightScale = 1.0f;
        public float minResolutionWidthScale = 0.5f;
        public float minResolutionHeightScale = 0.5f;
        public float scaleWidthIncrement = 0.1f;
        public float scaleHeightIncrement = 0.1f;

        float m_widthScale = 1.0f;
        float m_heightScale = 1.0f;

        // Variables for dynamic resolution algorithm that persist across frames
        uint m_frameCount = 0;

        const uint kNumFrameTimings = 3;

        double m_gpuFrameTime;
        double m_cpuFrameTime;

        // Use this for initialization
        private void Start()
        {
            int rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
            int rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
            _screenText.text = string.Format("Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}\n",
                m_widthScale,
                m_heightScale,
                rezWidth,
                rezHeight);
            _incrementBtn.onClick.AddListener(Increment);
            _decrementBtn.onClick.AddListener(Decrement);
        }

        // Update is called once per frame
        private void Update()
        {
            var oldWidthScale = m_widthScale;
            var oldHeightScale = m_heightScale;

            if (m_widthScale != oldWidthScale || m_heightScale != oldHeightScale)
                ScalableBufferManager.ResizeBuffers(m_widthScale, m_heightScale);
            
            DetermineResolution();
            
            var rezWidth = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * Screen.currentResolution.width);
            var rezHeight = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * Screen.currentResolution.height);
            _screenText.text = string.Format("Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}\nScaleFactor: {4:F3}x{5:F3}\nGPU: {6:F9} CPU: {7:F9}",
                m_widthScale,
                m_heightScale,
                rezWidth,
                rezHeight,
                ScalableBufferManager.widthScaleFactor,
                ScalableBufferManager.heightScaleFactor,
                m_gpuFrameTime,
                m_cpuFrameTime);
            
            // _screenText.text = string.Format(
            //     "Scale: {0:F3}x{1:F3}\nResolution: {2}x{3}",
            //     m_widthScale, m_heightScale,
            //     Screen.width, Screen.height);
        }

        private void Increment()
        {
            m_heightScale = Mathf.Max(minResolutionHeightScale, m_heightScale - scaleHeightIncrement);
            m_widthScale = Mathf.Max(minResolutionWidthScale, m_widthScale - scaleWidthIncrement);
        }

        private void Decrement()
        {
            m_heightScale = Mathf.Min(maxResolutionHeightScale, m_heightScale + scaleHeightIncrement);
            m_widthScale = Mathf.Min(maxResolutionWidthScale, m_widthScale + scaleWidthIncrement);
        }
        
        // Estimate the next frame time and update the resolution scale if necessary.
        private void DetermineResolution()
        {
            ++m_frameCount;
            if (m_frameCount <= kNumFrameTimings)
            {
                return;
            }
            FrameTimingManager.CaptureFrameTimings();
            FrameTimingManager.GetLatestTimings(kNumFrameTimings, frameTimings);
            if (frameTimings.Length < kNumFrameTimings)
            {
                Debug.LogFormat("Skipping frame {0}, didn't get enough frame timings.",
                    m_frameCount);

                return;
            }
            m_gpuFrameTime = frameTimings[0].gpuFrameTime;
            m_cpuFrameTime = frameTimings[0].cpuFrameTime;
            // Debug.Log($"Cpu time: {m_cpuFrameTime}, gpu time: {m_gpuFrameTime}");

        }
        
    }
}