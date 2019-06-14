using CAST.FileSystem;
using CAST.Loader;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CAST
{
    public class MODManager : MonoBehaviour
    {
        public static FileSystemStorage fileSystem;

        //SceneFirstDownload#Start() で一番最初に呼び出される
        public static void InitMODManager()
        {
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

        public static void InitPoseData()
        {
            Debug.Log("\n InitPoseData ");
            PoseDataLoader.LoadPoseData();
        }

        void Start()
        {
            SceneManager.sceneLoaded += onSceneLoaded;
        }

        void onSceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            if(nextScene.name.IndexOf("Main") != -1)
            {
                //カメラのy軸制限を解除
                GameMain.Instance.MainCamera.GetComponent<UltimateOrbitCamera>().limitY = false;
            }
        }
    }
}
