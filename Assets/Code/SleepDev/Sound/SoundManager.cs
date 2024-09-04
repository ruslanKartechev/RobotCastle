using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SleepDev
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {

        [Header("MIXERS")]
        [SerializeField] private Transform _parent;
        [SerializeField] private int _startSourcesCount = 50;
        private Queue<AudioSource> _sources;
        private AudioSource _musicSource;
        private List<AudioSource> _playingSources;

        private float _volume = 1f;
        private bool _isOn = true;
        private bool _didInit;

        protected float StatusMod => _isOn ? 1f : 0f;
        public float Volume => _volume;
        public bool IsOn => _isOn;
        

        public void Init(bool isOn, float volume)
        {
            CLog.LogWhite($"[SoundManager] Init {isOn}, {volume}");
            if (_didInit)
                return;
            _didInit = true;
            _isOn = isOn;
            _volume = volume;
            SoundContainer.SoundManager = this;
            DontDestroyOnLoad(gameObject);
            _sources = new Queue<AudioSource>(_startSourcesCount);
            for (byte i = 0; i < _startSourcesCount; i++)
            {
                var s = _parent.gameObject.AddComponent<AudioSource>();
                s.playOnAwake = false;
                _sources.Enqueue(s);
            }
            _playingSources = new List<AudioSource>(_startSourcesCount);
            _musicSource = _parent.gameObject.AddComponent<AudioSource>();
            BeginCheck();
        }
        
        public void SetStatus(bool onOff)
        {
            _isOn = onOff;
            CLog.LogWhite($"[SoundManager] Status Set to {_isOn}");
            foreach (var source in _playingSources)
                source.volume *= StatusMod;
            _musicSource.volume *= StatusMod;
        }

        public PlayingSound PlayMusic(SoundID sound, bool loop)
        {
            CLog.LogWhite($"ID {sound.clip.name}");
            _musicSource.clip = sound.clip;
            _musicSource.volume = sound.volume * Volume * StatusMod;
            _musicSource.loop = loop;
            _musicSource.Play();
            return new PlayingSound(_musicSource);
        }

        public PlayingSound Play(SoundID sound, bool loop)
        {
            var source = GetSource();
            var ps = new PlayingSound(source);
            source.clip = sound.clip;
            source.volume = Volume * sound.volume * StatusMod;
            source.loop = loop;
            source.Play();
            _playingSources.Add(source);
            return ps;
        }

        public PlayingSound Play(SoundID sound, bool loop, float volume)
        {
            var source = GetSource();
            var ps = new PlayingSound(source);
            source.clip = sound.clip;
            source.volume = Volume * sound.volume * volume * StatusMod;
            source.loop = loop;
            source.Play();
            _playingSources.Add(source);
            return ps;
        }

        public PlayingSound Play(SoundID sound, bool loop, float volume, float pitch)
        {
            var source = GetSource();
            var ps = new PlayingSound(source);
            source.clip = sound.clip;
            source.volume = Volume * sound.volume * volume * StatusMod;
            source.pitch = pitch;
            source.loop = loop;
            source.Play();
            _playingSources.Add(source);
            return ps;
        }
        
        private AudioSource GetSource()
        {
            if (_sources.Count == 0)
            {
                return _parent.gameObject.AddComponent<AudioSource>();
            }
            return _sources.Dequeue();
        }

        private void BeginCheck()
        {
            StartCoroutine(PlayingSourceCounting());
        }

        private IEnumerator PlayingSourceCounting()
        {
            while (true)
            {
                var toRemove = new List<AudioSource>(5);
                foreach (var playingSource in _playingSources)
                {
                    if (playingSource.isPlaying == false)
                    {
                        toRemove.Add(playingSource);
                    }  
                }
                foreach (var source in toRemove)
                {
                    _playingSources.Remove(source);
                    _sources.Enqueue(source);
                }
                yield return null;
            }
        }
    }
}