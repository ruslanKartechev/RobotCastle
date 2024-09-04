using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace SleepDev
{
    public static class EnvironmentState
    {
        public class EnvData
        {
            public EnvData(string scene, string trailId, string iconId, bool isNight, string windParticles)
            {
                this.scene = scene;
                this.trailId = trailId;
                this.iconId = iconId;
                this.isNight = isNight;
                this.windParticles = windParticles;
            }

            public string scene;
            public string trailId;
            public string iconId;
            public string windParticles;
            public bool isNight;
        }

        public static EnvData[] Data =
        {
            new( "Loc_city", trailId:"trail_city", iconId:"VIS_UI_0City_Icon", isNight:false,"wind_city"), // city 0
            new("Loc_city_chase", trailId:"trail_city", iconId:"VIS_UI_1City_Chase_Icon", isNight:false, "wind_city"), // city chase 1
            new("Loc_city_night",trailId:"trail_city", iconId:"VIS_UI_2City_Night_Icon", isNight:true, "wind_city"), // city night 2
            new("Loc_desert", trailId:"trail_desert", iconId:"VIS_UI_3Desert_Icon", isNight:false, "wind_desert"), // desert 3
            new("Loc_desert_night", trailId:"trail_desert", iconId:"VIS_UI_4Desert_Night_Icon", isNight:true, "wind_desert"), // desert night 4 
            new("Loc_winter", trailId:"trail_winter", iconId:"VIS_UI_5Winter_Icon", isNight:false, "wind_winter"), // winter 5
            new("Loc_winter_night", trailId:"trail_winter", iconId:"VIS_UI_6Winter_Night_Icon", isNight:true, "wind_winter"), // winter night 6
            new("Loc_forest", trailId:"trail_city", iconId:"VIS_UI_7Forest_Icon", isNight:false, "wind_city"), // forest 7
            new("Loc_forest_night", trailId:"trail_city", iconId:"VIS_UI_8Forest_Night_Icon", isNight:true, "wind_city"), // forest night 8
            new("Loc_city_godzilla", trailId:"trail_city", iconId:"VIS_UI_0City_Icon", isNight:false, "wind_city"), // city godzilla 9
        };
        
        public static EnvData CurrentData => Data[CurrentIndex];
        public static byte CurrentIndex { get; set; }
        public static bool IsNight => CurrentData.isNight;
        public static string TrailId => CurrentData.trailId;
        public static string WinId => CurrentData.windParticles;
        public static Light CurrentGlobalLight { get; set; }

        public static GameObject WindParticlesPrefab => 
            Resources.Load<GameObject>($"Prefabs/FX/{WinId}");

        public static ParticleSystem VehicleTrailPrefab() =>
            Resources.Load<ParticleSystem>($"Prefabs/FX/{TrailId}");
        
        public static Sprite GetIconForScene(string scene)
        {
            foreach (var envData in Data)
            {
                if (scene == envData.scene)
                    return Resources.Load<Sprite>($"UI/{envData.iconId}");
            }   
            return null;
        }
    }
}