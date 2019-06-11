using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace CAST
{
    public class MODManager : MonoBehaviour
    {
        static String temp = "";

        //SceneFirstDownload#Start() で一番最初に呼び出される
        public static void InitMODManager()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<MODManager>();
        }

        public static void InitGameUty()
        {
            Util.setPrivateStaticField(typeof(GameUty), "m_FileSystem", new FileSystemAB());
            GameUty.UpdateFileSystemPath();
            GameUty.UpdateFileSystemPathOld();
        }

        void Start()
        {
            SceneManager.sceneLoaded += onSceneLoaded;
        }

        void OnGUI()
        {
            GUI.TextArea(new Rect(10, 10, 300, 100), temp);
        }

        void onSceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            temp += nextScene.name + ":" + nextScene.isLoaded;
            if(nextScene.name.IndexOf("Main") != -1)
            {
                //カメラのy軸制限を解除
                GameMain.Instance.MainCamera.GetComponent<UltimateOrbitCamera>().limitY = false;
                var abFileSystem = GameUty.FileSystem as FileSystemAB;
            }
        }
    }
}
