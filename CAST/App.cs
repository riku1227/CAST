using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAST
{
    public class App
    {
        public static string base_path = "CAST/";
        public static string pose_csv_path = "pose_data";
        public static string parts_mlist_path = "parts_data";
        public static string bg_csv_path = "bg_data";
        public static string position_preset_path = "position_preset";
        public static string wear_preset_path = "wear_preset";
        public static FileSystemAB fileSystemAB = null;

        public static int min = 0;

        public class Config
        {
            public static bool showLog = false;
            public static bool showFPS = false;
            public static bool oldFaceChangerUI = false;
            public static bool lowAngle = false;
            public static bool allowOrientation = false;
            public static bool showFaceChanger = false;
            public static bool showPositionChanger = false;
            public static bool showMaidEditor = false;
            public static bool showLightSetting = false;
            public static int showExPoseSelect = 0;
            public static bool showAdvancedColorPalette = false;
            public static bool showHideAllGUIButton = false;
        }
    }
}
