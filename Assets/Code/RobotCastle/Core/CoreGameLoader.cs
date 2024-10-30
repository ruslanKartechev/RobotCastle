using System.Collections;
using MadPixel.InApps;
using MAXHelper;
using RobotCastle.MainMenu;
using RobotCastle.Saving;
using RobotCastle.Shop;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Core
{
    [DefaultExecutionOrder(-300)]
    public class CoreGameLoader : MonoBehaviour
    {
        public static bool DidInit;
        [Header("Core")]
        [SerializeField] private bool _autoStart = true;
        [SerializeField] private bool _initSdk = true;
        [SerializeField] private DataInitializer _dataInitializer;
        [SerializeField] private GameInput _gameInput;
        [Header("In app data")]
        [SerializeField] private bool _logSkus;
        [SerializeField] private InAppsSKUDataBase _inAppsData;
        [Header("Ads")]
        [SerializeField] private AdsPlayer.AdPlayMode _adPlayMode;
        [SerializeField] private float _intersReloadtime = 30f;
        

#if UNITY_EDITOR
        private void OnValidate()
        {
            NamingDataLoader.CreateIfNone();
        }

        [ContextMenu("ForceRewriteNamingData")]
        public void ForceRewriteNamingData()
        {
            NamingDataLoader.ForceWriteNew();
        }
        #endif

        private void Awake()
        {
            if(!DidInit && _autoStart)
                Init();
        }

        public void Init()
        {
            if (DidInit) return;
            DidInit = true;
            DontDestroyOnLoad(gameObject);
            NamingDataLoader.Load();
            ServiceLocator.Create();
            var dataSaver = DataSaver.Create();
            ServiceLocator.Bind<IDataSaver>(dataSaver);
            ServiceLocator.Bind<DataSaver>(dataSaver);
            var uiManager = UIManager.Create();
            ServiceLocator.Bind<IUIManager>(uiManager);
            ServiceLocator.Bind<UIManager>(uiManager);
            ServiceLocator.Bind<SceneLoader>(SceneLoader.Create());
            ServiceLocator.Bind<GameInput>(_gameInput);
            _gameInput.Init();
            CLog.Log($"Loading saves");
            _dataInitializer.LoadAll();
            AdsPlayer.Create(_intersReloadtime, _adPlayMode);
            
            CLog.Log($"Calling other loaders");
            var loaders = GetComponentsInChildren<IGameLoader>();
            foreach (var ll in loaders)
                ll.Load();
            ServiceLocator.Bind<GameMoney>(GameMoney.Create());
            ServiceLocator.Bind<CastleXpManager>(CastleXpManager.Create());
            ServiceLocator.Bind<PlayerEnergyManager>(PlayerEnergyManager.Create());
            
            _inAppsData.Init();
            ServiceLocator.Bind<InAppsSKUDataBase>(_inAppsData);
            if(_initSdk)
                InitSdk();
            InputBtn.Create();
        }


        public void InitSdk()
        {
            CLog.Log($"Loading SDK");

            var path = "prefabs/sdk_components";
            var prefab = Resources.Load<GameObject>(path);
            var inst = Instantiate(prefab);
            DontDestroyOnLoad(inst);
            StartCoroutine(SdkInit(inst));
        }

        // Ads, Analytics, Purchaser
        private IEnumerator SdkInit(GameObject go)
        {
            yield return null;
            var ads = go.transform.GetChild(0).GetComponent<AdsManager>();
            ads.InitApplovin();
            yield return null;
            var analytics = go.transform.GetChild(1).GetComponent<MadPixelAnalytics.AnalyticsManager>();
            analytics.Init();
            analytics.SubscribeToAdsManager();
            
            var purchaser = go.transform.GetComponentInChildren<MobileInAppPurchaser>();
            if (purchaser == null)
            {
                CLog.LogRed($"IN APP PURCHASER IS NULL!");
                yield break;
            }
            
            if (_logSkus)
            {
                foreach (var ss in _inAppsData.consumables)
                    CLog.Log($"Consumable: {ss}");    
                foreach (var ss in _inAppsData.nonConsumables)
                    CLog.Log($"NonConsumable: {ss}");    
            }
            // purchaser.OnPurchaseResult += OnPurchaseResult;
            purchaser.Init(_inAppsData.nonConsumables, _inAppsData.consumables);
        }
        
    }
}