using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace CAST.UI
{
    class LogUI : MonoBehaviour
    {
        private string logText = "";
        private string castLogText = "";
        private string oldLogText = "";
        private bool first = false;
        //private static int logCount = 5;

        public static GameObject objectInstance = null;
        public static void Init()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<LogUI>();
            objectInstance = go;
        }

        private void Awake()
        {
            Application.logMessageReceived += OnLogMessage;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived += OnLogMessage;
        }

        void OnGUI()
        {
            if (App.Config.showLog)
            {
                var fps = 0;
                if(App.Config.showFPS)
                {
                    fps = 160;
                }
                
                GUI.TextArea(new Rect(10, 10, 300, 750), logText);

                GUI.TextArea(new Rect(Screen.width - 310, 10 + fps, 300, 500), castLogText);
            }
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            Debug.Log("[CAST] SceneChange:" + arg1);
        }

        private void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (string.IsNullOrEmpty(condition))
            {
                return;
            }

            if (condition.IndexOf("[CAST]") != -1)
            {
                castLogText += condition + "\n";
            }
            else
            {
                if(!first)
                {
                    oldLogText = condition;
                    logText += condition + "\n";
                    first = true;
                }
                else
                {
                    if (condition != "NullReferenceException: Object reference not set to an instance of an object" && condition != "String too long for TextMeshGenerator. Cutting off characters." && condition.IndexOf("poseicon") == -1 && condition.IndexOf("GUI.Draw") == -1 && condition.IndexOf("IndexOutOfRange") == -1)
                    {
                        logText += condition + "\n";
                    }
                }
            }
        }
    }
}
