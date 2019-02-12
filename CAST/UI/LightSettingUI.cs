using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CAST.UI
{
    public class LightSettingUI : MonoBehaviour
    {
        float screenBaseWidth = 0F;
        float screenBaseHeight = 0F;

        float changeIntensityValue = 0.01F;
        float changeColorValue = 0.1F;

        bool isLandscape = false;
        public static GameObject Init()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<LightSettingUI>();
            return go;
        }

        void OnGUI()
        {
            if(App.Config.showLightSetting)
            {
                if (Screen.width > Screen.height)
                {
                    screenBaseWidth = Screen.height / 100;
                    screenBaseHeight = Screen.width / 100;
                    isLandscape = true;
                }
                else
                {
                    screenBaseWidth = Screen.width / 100;
                    screenBaseHeight = Screen.height / 100;
                    isLandscape = false;
                }

                GUI.Window(57859868, new Rect(10, screenBaseHeight * 30, screenBaseWidth * 38, screenBaseHeight * 14), onSettingWindow, "ライト設定");
                
                if(isLandscape)
                {
                    GUI.Window(57859868, new Rect(10 + screenBaseWidth * 38 + 10, screenBaseHeight * 30, screenBaseWidth * 38, screenBaseHeight * 20), onColorSettingWindow, "ライトカラー設定");
                }
                else
                {
                    GUI.Window(15467492, new Rect(10, screenBaseHeight * 30 + screenBaseHeight * 14 + 10, screenBaseWidth * 38, screenBaseHeight * 20), onColorSettingWindow, "ライトカラー設定");
                }
            }
        }

        private void onSettingWindow(int id)
        {
            var editStyle = GUI.skin.GetStyle("textField");
            editStyle.fontSize = 40;
            editStyle.alignment = TextAnchor.MiddleCenter;
            var windowWBase = screenBaseWidth * 38 / 100;
            var windowHBase = screenBaseHeight * 20 / 100;

            LightMain lightMain = GameMain.Instance.MainLight;

            changeIntensityValue = float.Parse(GUI.TextField(new Rect(10, 30, windowWBase * 100 - 20, windowHBase * 15), changeIntensityValue.ToString(), editStyle));
            if (GUI.Button(new Rect(10, 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "+"))
            {
                lightMain.SetIntensity(lightMain.GetIntensity() + changeIntensityValue);
            }
            lightMain.SetIntensity(float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (40 + windowHBase * 15), windowWBase * 50, windowHBase * 20), lightMain.GetIntensity().ToString(), editStyle)));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "-"))
            {
                lightMain.SetIntensity(lightMain.GetIntensity() - changeIntensityValue);
            }

            if (GUI.Button(new Rect(10, 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "+"))
            {
                lightMain.SetShadowStrength(lightMain.GetShadowStrength() + changeIntensityValue);
            }
            lightMain.SetShadowStrength(float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (50 + windowHBase * 15 + (windowHBase * 20)), windowWBase * 50, windowHBase * 20), lightMain.GetShadowStrength().ToString(), editStyle)));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "-"))
            {
                lightMain.SetShadowStrength(lightMain.GetShadowStrength() - changeIntensityValue);
            }
        }

        private void onColorSettingWindow(int id)
        {
            var editStyle = GUI.skin.GetStyle("textField");
            editStyle.fontSize = 40;
            editStyle.alignment = TextAnchor.MiddleCenter;
            var windowWBase = screenBaseWidth * 38 / 100;
            var windowHBase = screenBaseHeight * 20 / 100;
            var labelStyle = GUI.skin.label;
            labelStyle.fontSize = 20;

            changeColorValue = float.Parse(GUI.TextField(new Rect(10, 30, windowWBase * 100 - 20, windowHBase * 15), changeColorValue.ToString(), editStyle));

            LightMain lightMain = GameMain.Instance.MainLight;
            var color = lightMain.GetColor();

            GUI.Label(new Rect(10, 40 + windowHBase * 15, windowWBase * 5, windowHBase * 5), "R", labelStyle);
            if (GUI.Button(new Rect(10 + windowWBase * 5, 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "+"))
            {
                color.r += changeColorValue;
            }
            color.r = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20 + windowWBase * 5), (40 + windowHBase * 15), windowWBase * 45, windowHBase * 20), color.r.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "-"))
            {
                color.r -= changeColorValue;
            }

            GUI.Label(new Rect(10, 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 5, windowHBase * 5), "G", labelStyle);
            if (GUI.Button(new Rect(10+ windowWBase * 5, 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "+"))
            {
                color.g += changeColorValue;
            }
            color.g = float.Parse(GUI.TextField(new Rect(((10 * 2)  + windowWBase * 20 + windowWBase * 5), (50 + windowHBase * 15 + (windowHBase * 20)), windowWBase * 45, windowHBase * 20), color.g.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "-"))
            {
                color.g -= changeColorValue;
            }

            GUI.Label(new Rect(10, 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 5, windowHBase * 5), "B", labelStyle);
            if (GUI.Button(new Rect(10 + windowWBase * 5, 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 20, windowHBase * 20), "+"))
            {
                color.b += changeColorValue;
            }
            color.b = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20 + windowWBase * 5), (60 + windowHBase * 15 + (windowHBase * 20 * 2)), windowWBase * 45, windowHBase * 20), color.b.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 20, windowHBase * 20), "-"))
            {
                color.b -= changeColorValue;
            }

            lightMain.SetColor(color);
        }
    }
}
