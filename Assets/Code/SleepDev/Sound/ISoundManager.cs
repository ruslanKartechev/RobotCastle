namespace SleepDev
{
    public interface ISoundManager
    {
        void Init(bool isOn, float volume, bool musicOn, float musicVolume);
        
        PlayingSound Play(SoundID sound, bool loop);
        PlayingSound Play(SoundID sound, bool loop, float volume);
        PlayingSound Play(SoundID sound, bool loop, float volume, float pitch);

        float Volume { get; }
        bool IsOn { get; }
        void SetStatusSound(bool onOff);
        void SetStatusMusic(bool onOff);
        
        PlayingSound PlayMusic(SoundID sound, bool loop);
    }
}