using System.Collections.Generic;

namespace RobotCastle.Saving
{
    public class NamingData
    {
        private static NamingData _inst;

        public static NamingData Inst => _inst;
        
        public static NamingData Set(NamingData data)
        {
            _inst = data;
            return _inst;
        }
        
        public static NamingData GetSample()
        {
            var data = new NamingData();
            data.uiData.Add("win", "scene_ui_win");
            data.uiData.Add("fail", "scene_ui_fail");
            data.tutorialsData.Add("tutor_1", "vid_tutor_1");
            data.tutorialsData.Add("tutor_2", "vid_tutor_2");
            return data;
        }

        public Dictionary<string, string> uiData = new Dictionary<string, string>(30);
        public Dictionary<string, string> tutorialsData = new Dictionary<string, string>(30);

        
  

    }
}