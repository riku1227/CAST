using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Profiling;

namespace CAST.UI
{
    class FPSCounterUI : MonoBehaviour
    {
        private float fps = 0;
        private float allocatedRam = 0;
        private float monoRam = 0;
        private int frameCount;
        private float prevTime;
        public static GameObject objectInstance = null;

        public static void Init()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<FPSCounterUI>();
            objectInstance = go;
        }

        void Start()
        {
            frameCount = 0;
            prevTime = 0.0f;
        }

        void Update()
        {
            ++frameCount;
            float time = Time.realtimeSinceStartup - prevTime;

            if (time >= 0.5f)
            {
                //Debug.LogFormat("{0}fps", frameCount / time);
                fps = frameCount / time;
                frameCount = 0;
                prevTime = Time.realtimeSinceStartup;
            }

            allocatedRam = Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
            monoRam = Profiler.GetTotalReservedMemoryLong() / 1048576f;
        }

        void OnGUI()
        {
            if(App.Config.showFPS)
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = 50;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.black;
                GUI.Label(new Rect(Screen.width - 310, 10, 300, 50), Convert.ToString(Convert.ToInt32(fps)) + "FPS", style);

                GUI.Label(new Rect(Screen.width - 610, 70, 600, 50), "ReservedRAM: " + Convert.ToString(monoRam) + "MB", style);
                GUI.Label(new Rect(Screen.width - 610, 130, 600, 50), "AllocatedRAM: " + Convert.ToString(allocatedRam) + "MB", style);
            }
        }
    }
}
