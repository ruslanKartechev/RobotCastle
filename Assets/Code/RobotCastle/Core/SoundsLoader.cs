using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Core
{
    public class SoundsLoader : MonoBehaviour, IGameLoader
    {
        [SerializeField] private SoundID _music;
        [SerializeField] private SoundManager _soundManager;
        
        public void Load()
        {
            var data = DataHelpers.GetPlayerData();
            _soundManager.Init(data.soundOn, 1f, data.musicOn, 1f);
            _soundManager.PlayMusic(_music, true);
            
        }
        
    }
}