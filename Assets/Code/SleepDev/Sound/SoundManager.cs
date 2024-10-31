using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        public static SoundManager Inst => _inst;
        
        public float Volume => _volume;
        public bool IsOn => _isOnSound;
        
        public void Init(bool isOn, float volume, bool musicOn, float musicVolume)
        {
            CLog.LogWhite($"[SoundManager] Init {isOn}, {volume}");
            if (_didInit)
                return;
            _inst = this;
            _didInit = true;
            _isOnSound = isOn;
            _isOnMusic = musicOn;
            _volume = volume;
            _volumeMusic = musicVolume;
            SoundContainer.SoundManager = this;
            transform.parent = null;
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
        
        public void SetStatusSound(bool onOff)
        {
            _isOnSound = onOff;
            foreach (var source in _playingSources)
            {
                source.mute = !onOff;
            }
        }
        
        public void SetStatusMusic(bool onOff)
        {
            _isOnMusic = onOff;
            _musicSource.mute = !onOff;
        }

        public PlayingSound PlayMusic(SoundID sound, bool loop)
        {
            _musicSource.clip = sound.clip;
            _musicSource.volume = sound.volume * _volumeMusic * MusicStatusMod;
            _musicSource.loop = loop;
            _musicSource.Play();
            return new PlayingSound(_musicSource);
        }

        public PlayingSound Play(SoundID sound, bool loop = false)
        {
            var source = GetSource();
            var ps = new PlayingSound(source);
            source.clip = sound.clip;
            source.volume = Volume * sound.volume * SoundStatusMod;
            source.loop = loop;
            source.Play();
            source.mute = !_isOnSound;
            _playingSources.Add(source);
            return ps;
        }

        public PlayingSound Play(SoundID sound, bool loop, float volume)
        {
            var source = GetSource();
            var ps = new PlayingSound(source);
            source.clip = sound.clip;
            source.volume = Volume * sound.volume * volume * SoundStatusMod;
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
            source.volume = Volume * sound.volume * volume * SoundStatusMod;
            source.pitch = pitch;
            source.loop = loop;
            source.Play();
            _playingSources.Add(source);
            return ps;
        }
        
        private static SoundManager _inst;
        
        [SerializeField] private Transform _parent;
        [SerializeField] private int _startSourcesCount = 50;
        private Queue<AudioSource> _sources;
        private AudioSource _musicSource;
        private List<AudioSource> _playingSources;

        private float _volume = 1f;
        private float _volumeMusic = 1f;
        private bool _isOnSound = true;
        private bool _isOnMusic = true;
        
        private bool _didInit;

        protected float SoundStatusMod => _isOnSound ? 1f : 0f;
        protected float MusicStatusMod => _isOnMusic ? 1f : 0f;

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