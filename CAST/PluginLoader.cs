using CAST.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;

using SCENE_EDIT;
using System.IO;
using System.Reflection;

namespace CAST
{
    public class PluginLoader : MonoBehaviour
    {

        private static GameObject faceChanger = null;
        private static bool initConfigUI = false;

        private static AssetBundle shaderBundle;

        static void InitPluginLoader()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<PluginLoader>();
        }

        private void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            App.Config.showLog = true;
            LogUI.Init();
            UI.ConfigUI.Init();
            initConfigUI = true;
            UI.ConfigUI.show = true;
        }

        private void Update()
        {

            if(FPSCounterUI.objectInstance == null)
            {
                if(App.Config.showFPS)
                {
                    FPSCounterUI.Init();
                }
            }
        }

        private void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if(nextScene.name == "SceneMain")
            {
                App.Config.showLog = false;
                var cm = GameMain.Instance.CharacterMgr;
                var list = cm.m_listStockMaid;
                bool first = true;
                foreach(Maid maid in list)
                {
                    if(first)
                    {
                        first = false;
                    }
                    else
                    {
                        cm.DeactivateMaid(maid);
                    }
                }
                if(!initConfigUI)
                {
                    UI.ConfigUI.Init();
                    initConfigUI = true;
                } else
                {
                    UI.ConfigUI.show = true;
                }
            } else if (nextScene.name == "SceneEdit")
            {
                UI.ConfigUI.show = false;
                Camera.main.farClipPlane = Camera.main.farClipPlane * 10;
            }

            FaceChanger.setShow(false);
        }

        public static void InitEdit()
        {
            Debug.Log("[CAST] InitEdit");
            MaidEditor.Init();
            if (faceChanger == null)
            {
                faceChanger = FaceChanger.Init();
                PositionChanger.Init();
                LightSettingUI.Init();
            }
        }

        public static void OnClickExitAndSave()
        {
            FaceChanger.setShow(false);
            PositionChanger.setShow(false);
            MaidEditor.setShow(false);
            App.Config.showExPoseSelect = 0;
            App.Config.showLightSetting = false;
            UI.ConfigUI.show = false;
        }

        public static void OnClickOption()
        {
            FaceChanger.setShow(false);
            PositionChanger.setShow(false);
            MaidEditor.setShow(false);
            App.Config.showExPoseSelect = 0;
            App.Config.showLightSetting = true;
            UI.ConfigUI.show = true;
        }

        public static void OnClickView()
        {
            FaceChanger.setShow(false);
            PositionChanger.setShow(false);
            MaidEditor.setShow(true);
            App.Config.showExPoseSelect = 0;
            App.Config.showLightSetting = false;
            UI.ConfigUI.show = false;
        }

        public static void OnClickBg()
        {
            FaceChanger.setShow(false);
            PositionChanger.setShow(false);
            MaidEditor.setShow(false);
            App.Config.showExPoseSelect = 0;
            App.Config.showLightSetting = false;
            UI.ConfigUI.show = false;
        }

        public static void OnClickCustomize()
        {
            FaceChanger.setShow(false);
            PositionChanger.setShow(false);
            MaidEditor.setShow(false);
            App.Config.showExPoseSelect = 0;
            App.Config.showLightSetting = false;
            UI.ConfigUI.show = false;
        }

        public static void OnPoseWindowOpen()
        {
            FaceChanger.setShow(true);
            PositionChanger.setShow(false);
            MaidEditor.setShow(false);
            App.Config.showExPoseSelect = 0;
            App.Config.showLightSetting = false;
            UI.ConfigUI.show = false;
        }

        public static void OnPoseWindowClose()
        {
            FaceChanger.setShow(false);
            PositionChanger.setShow(false);
            MaidEditor.setShow(false);
            if (App.Config.showExPoseSelect == 1)
            {
                App.Config.showExPoseSelect = 2;
                FaceChanger.setShow(true);
            }
            App.Config.showLightSetting = false;
            UI.ConfigUI.show = false;
        }

        public static void OnOpenColorPalette(MaidParts.PARTS_COLOR type)
        {
            if(!AdvancedColorEdit.init)
            {
                AdvancedColorEdit.Init();
            }

            AdvancedColorEdit.nowType = type;
            App.Config.showAdvancedColorPalette = true;
        }
    }
}
