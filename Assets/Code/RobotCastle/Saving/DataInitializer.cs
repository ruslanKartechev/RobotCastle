using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;
#if UNITY_EDITOR
using System.Diagnostics;
#endif

namespace RobotCastle.Saving
{
    [CreateAssetMenu(menuName = "SO/Data Initializer", fileName = "Data Initializer", order = -200)]
    public class DataInitializer : ScriptableObject
    {
        [SerializeField] private bool _clearOnStart;
        [SerializeField] private bool _useCheat;
        [SerializeField] private CheatManager _cheatManager;
        [SerializeField] private SavesDataBase _initialSaves;

        public void LoadAll()
        {
            var saver = ServiceLocator.Get<DataSaver>();
            if (_clearOnStart)
            {
                saver.Delete<SavePlayerData>();
                saver.Delete<SaveLevelsProgress>();
                saver.Delete<SavePlayerHeroes>();
            }
            saver.LoadSave<SavePlayerData>(new SavePlayerData(_initialSaves.PlayerData));
            saver.LoadSave<SaveLevelsProgress>(new SaveLevelsProgress(_initialSaves.LevelsProgress));
            saver.LoadSave<SavePlayerHeroes>(new SavePlayerHeroes(_initialSaves.PlayerHeroes));
            if (_useCheat)
                _cheatManager.ApplyStartCheat();
        }

        [ContextMenu("Clear All Save Files")]
        public void ClearAllSaveFiles()
        {
            DataSaver.DeleteFile<SavePlayerData>();
            DataSaver.DeleteFile<SavePlayerData>();
            DataSaver.DeleteFile<SaveLevelsProgress>();
            DataSaver.DeleteFile<SavePlayerHeroes>();
        }

        [ContextMenu("Log Saves Path")]
        public void LogSavesPath()
        {
            CLog.LogGreen($"{Application.persistentDataPath}");
#if UNITY_EDITOR
            Process.Start("explorer.exe", 
                Application.persistentDataPath.Replace('/', '\\'));
#endif
        }
    }
}