using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class TextFiller : MonoBehaviour
    {

        public string Msg
        {
            get => _msg;
            set => _msg = value;
        }

        public void AnimateText(string msg, Action callback)
        {
            StopFilling();
            _msg = msg;
            _text.text = "";
            var length = msg.Length;
            if (length == 0)
                return;
            var delay = _perCharDefault;
            foreach (var dd in _perCharDelays)
            {
                if (length <= dd.charsCount)
                {
                    delay = dd.delayPerChar;
                    break;
                }
            }
            _filling = StartCoroutine(TextFilling(delay, callback));
        }

        public void SetText(string msg)
        {
            _msg = msg;
            _text.text = msg;
        }

        public void StopFilling()
        {
            if(_filling != null)
                StopCoroutine(_filling);
        }
        
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _perCharDefault = .02f;
        [SerializeField] private int _singleLineCharsCount = 10;
        [SerializeField] private string _colorHex;
        [SerializeField] private float _nextLineDelay = .2f;
        [SerializeField] private List<DelayData> _perCharDelays;
        private string _msg;
        private Coroutine _filling;
        
        private IEnumerator TextFilling(float delay, Action callback)
        {
            var length = _msg.Length;
            if (length <= _singleLineCharsCount)
                _text.alignment = TextAlignmentOptions.Center;
            else
            {
                _text.verticalAlignment = VerticalAlignmentOptions.Top;
                _text.horizontalAlignment = HorizontalAlignmentOptions.Left;
            }

            var lines = _msg.Split("<br>");
            var fullStr = "";
            // Debug.Log($"======= Lines count: {lines.Length}");
            foreach (var line in lines)
            {
                string fullLine;
                var lineLength = line.Length;
                // Debug.Log($"======= Line: {line}");
                for (var i = 0; i < lineLength; i++)
                {
                    var len = i + 1;
                    var s1 = line.Substring(0, len);
                    var s2 = line.Substring(len, lineLength - len);
                    s1 = $"<color={_colorHex}>{s1}</color>";
                    s2 = $"<color=#00000000>{s2}</color>";
                    fullLine = s1 + s2;
                    _text.text = fullStr + fullLine;
                    yield return new WaitForSeconds(delay);
                }
                fullStr += line + "<br>";
                yield return new WaitForSeconds(_nextLineDelay);
            }
            callback?.Invoke();
        }
        
        

        [System.Serializable]
        private class DelayData
        {
            public int charsCount;
            public float delayPerChar;
        }
    }
}