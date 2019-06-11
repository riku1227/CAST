using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace CAST.UI
{
    class LogUI : MonoBehaviour
    {
        private String logText = "";
        public static void Init()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<LogUI>();
        }

        void Awake()
        {
            Application.logMessageReceived += OnLogMessage;
        }

        void OnGUI()
        {
            GUI.TextArea(new Rect(Screen.width - 350 - 10, 10, 350, 600), logText);
        }

        void OnLogMessage(String logText, String stackTrace, LogType logType)
        {
            this.logText += logText + "\n";
        }
    }
}
