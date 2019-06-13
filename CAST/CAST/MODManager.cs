using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace CAST
{
    public class MODManager : MonoBehaviour
    {
        static String temp = "";
        static FileSystemStorage fileSystem;

        //SceneFirstDownload#Start() で一番最初に呼び出される
        public static void InitMODManager()
        {
            var harmony = HarmonyInstance.Create("com.riku1227.cast");
            //harmony.PatchAll(Assembly.GetExecutingAssembly());
            UI.LogUI.Init();
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<MODManager>();
        }

        public static void InitGameUty()
        {
            fileSystem = new FileSystemStorage();
            var assetLoader = new AssetLoader(fileSystem);
            var modDirectory = AFileSystemBase.base_path + "CAST/";
            assetLoader.loadDirectory(modDirectory);
            
            Util.setPrivateStaticField(typeof(GameUty), "m_FileSystem", fileSystem);
            GameUty.UpdateFileSystemPath();
            GameUty.UpdateFileSystemPathOld();
        }

        void Start()
        {
            SceneManager.sceneLoaded += onSceneLoaded;
        }

        void onSceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            temp += nextScene.name + ":" + nextScene.isLoaded;
            if(nextScene.name.IndexOf("Main") != -1)
            {
                //カメラのy軸制限を解除
                GameMain.Instance.MainCamera.GetComponent<UltimateOrbitCamera>().limitY = false;

                //uGUISystemDialog.OpenYesNo("TestDialog", delegate() { }, delegate () { });
            }
        }
    }

    [HarmonyPatch (typeof(String))]
    [HarmonyPatch ("ToUpper")]
    public class Patch
    {
        static void Prefix()
        {
            Debug.Log("ToUpper");
        }
    }
}
