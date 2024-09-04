using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public interface IMusicPlayer
    {
        void BeginPlaying();
        void Stop();
        void Mute(bool mute);
    }
    public class SimpleMusicPlayer : MonoBehaviour, IMusicPlayer
    {
        [SerializeField] private List<SoundID> _soundIds;

        private int _index;
        private Coroutine _playing;
        private PlayingSound _playingSound;
        private bool _isMuted = false;

        private void Awake()
        {
            SoundContainer.MusicPlayer = this;
        }

        public void BeginPlaying()
        {
            if (_soundIds.Count == 0)
                return;
            _index = Mathf.Clamp(_index, 0, _soundIds.Count - 1);
            Stop();
            _playing = StartCoroutine(Playing());
        }

        public void Stop()
        {
            if(_playing != null)
                StopCoroutine(_playing);
            _playingSound?.Stop();
        }

        public void Mute(bool mute)
        {
            _isMuted = mute;
            _playingSound?.Mute(_isMuted);
        }

        private IEnumerator Playing()
        {
            while (true)
            {
                _playingSound = SoundContainer.SoundManager.PlayMusic(_soundIds[_index], false);
                _playingSound.Mute(_isMuted);
                yield return new WaitForSeconds(_soundIds[_index].clip.length);
                _index++;
                _index = Mathf.Clamp(_index, 0, _soundIds.Count - 1);
                yield return null;
            }   
        }

    }
}