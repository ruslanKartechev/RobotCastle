using UnityEngine;

namespace SleepDev
{
    public class PlayingSound
    {
        private AudioSource _source;

        public PlayingSound(AudioSource source)
        {
            _source = source;
        }

        public void Mute(bool mute)
        {
            _source.mute = mute;
        }
        
        public void Stop()
        {
            _source.Stop();
        }

        public void SetLoop(bool loop)
        {
            _source.loop = loop;
        }

        public void SetVolume(float volume)
        {
            _source.volume = volume;
        }

    }
}