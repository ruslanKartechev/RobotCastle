using UnityEngine;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/Sound/SoundID", fileName = "SoundID", order = 0)]
    public class SoundID : ScriptableObject
    {
        public AudioClip clip;
        [Range(0f,1f)]  public float volume = 1f;
    }
}