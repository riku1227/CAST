using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CAST.UI
{
    public class HIdeAllGUIButton : MonoBehaviour
    {
        public static bool init = false;
        public static GameObject Init()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<HIdeAllGUIButton>();
            init = true;
            return go;
        }

        void OnGUI()
        {
            if(App.Config.showHideAllGUIButton)
            {
                if(GUI.Button(new Rect(Screen.width - 180 - 10, 10, 180, 180), "", new GUIStyle()))
                {
                    SceneEdit.Instance.MainMenuUI.OpenFast();
                    SceneEdit.Instance.OptionMenuUI.OpenFast();
                    GameMain.Instance.SNSShare.m_ShareButton.gameObject.SetActive(true);
                    App.Config.showHideAllGUIButton = false;
                }
            }
        }
    }
}
