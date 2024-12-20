﻿using UnityEngine;

namespace RobotCastle.Core
{
    [CreateAssetMenu(menuName = "SO/GlobalConfig", fileName = "GlobalConfig", order = 0)]
    public class GlobalConfig : ScriptableObject
    {
        public const string Rewarded_ScrollForAd = "scroll_for_ad";
        
        public const string SceneLoading = "Scene_loading";
        public const string SceneMainMenu = "Scene_main_menu";
        public const string SceneBattle = "Scene_battle";

        public const int MaxUnitsItemsCount = 3;


    }
}