using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

using SCENE_EDIT;

namespace CAST.UI
{
    public class ConfigUI : MonoBehaviour
    {
        public static bool show = true;
        private bool showWindow = false;
        private Texture2D texture = null;
        private ScreenOrientation defaultOrient = Screen.orientation;

        public static void Init()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<ConfigUI>();
        }

        private void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            /* if (arg1.name == "SceneMain")
            {
                show = true;
            } else
            {
                show = false;
            } */
        }

        void OnGUI()
        {
            if(show)
            {
                GUIStyle style = new GUIStyle();
                GUIStyleState styleState = new GUIStyleState();
                if (App.fileSystemAB != null)
                {
                    if (texture == null)
                    {
                        texture = GetTexture2D(App.fileSystemAB, "coloricon_pink_c.png");
                    }
                    styleState.background = texture;
                }
                style.fontSize = 50;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal = styleState;
                int padding = 0;
                if(App.Config.showLog)
                {
                    padding = 300;
                }
                if (GUI.Button(new Rect(Screen.width - 160 - padding, 115, 125, 75), "設定", style))
                {
                    Debug.Log("[CAST] Onclick");
                    showWindow = !showWindow;
                }

                if(showWindow)
                {
                    GUI.Window(13568945, new Rect(10, Screen.height / 2, Screen.width - 20, Screen.height / 3), OnWindow, "[CAST] 設定");
                }
            }
        }

        private void OnWindow(int id)
        {
            var buttonStyle = GUI.skin.GetStyle("button");
            buttonStyle.fontSize = 16;
            var baseRect = 20;
            var buttonWidth = 150;
            var buttonHeight = 75;

            //Log
            var logButtonStr = "";
            if(App.Config.showLog)
            {
                logButtonStr = "Logを非表示にする";
            } else
            {
                logButtonStr = "Logを表示する";
            }
            if(GUI.Button(new Rect(baseRect, baseRect, 225, 75), logButtonStr)) {
                Debug.Log("[CAST] OnclickLogButton");
                App.Config.showLog = !App.Config.showLog;
            }

            //FPS
            var fpsButtonStr = "";
            if (App.Config.showFPS)
            {
                fpsButtonStr = "FPSを非表示にする";
            }
            else
            {
                fpsButtonStr = "FPSを表示する";
            }
            if (GUI.Button(new Rect(baseRect, baseRect + buttonHeight + 10, 225, 75), fpsButtonStr))
            {
                Debug.Log("[CAST] OnclickFPSButton");
                App.Config.showFPS = !App.Config.showFPS;
            }

            //OldFaceChangerUI
            var oldFaceChangerUIStr = "";
            if (App.Config.oldFaceChangerUI)
            {
                oldFaceChangerUIStr = "古い表情変更を無効化する";
            }
            else
            {
                oldFaceChangerUIStr = "古い表情変更を有効化する";
            }
            if (GUI.Button(new Rect(baseRect, baseRect + (buttonHeight * 2) + (10 * 2), 225, 75), oldFaceChangerUIStr))
            {
                Debug.Log("[CAST] OldFaceChangerUIButton");
                App.Config.oldFaceChangerUI = !App.Config.oldFaceChangerUI;
                Debug.Log("Old" + App.Config.showFaceChanger.ToString());
            }

            //LowAngle
            var lowAngleUIStr = "";
            if (!App.Config.lowAngle)
            {
                lowAngleUIStr = "ローアングルを無効化";
            }
            else
            {
                lowAngleUIStr = "ローアングルを有効化";
            }
            if (GUI.Button(new Rect(baseRect, baseRect + (buttonHeight * 3) + (10 * 3), 225, 75), lowAngleUIStr))
            {
                Debug.Log("[CAST] OldFaceChangerUIButton");
                App.Config.lowAngle = !App.Config.lowAngle;
                GameMain.Instance.CameraMoveMgr._camera.limitY = App.Config.lowAngle;
            }

            //Orientation
            var allowOrientationStr = "";
            if (!App.Config.allowOrientation)
            {
                allowOrientationStr = "画面回転を有効化(不安定)";
            }
            else
            {
                allowOrientationStr = "画面回転を無効化";
            }
            if (GUI.Button(new Rect(baseRect, baseRect + (buttonHeight * 4) + (10 * 4), 225, 75), allowOrientationStr))
            {
                Debug.Log("[CAST] OldFaceChangerUIButton");
                App.Config.allowOrientation = !App.Config.allowOrientation;
                if(App.Config.allowOrientation)
                {
                    Screen.orientation = ScreenOrientation.AutoRotation;
                } else
                {
                    Screen.orientation = defaultOrient;
                }
                
            }

            //ExtractAssetBundle
            if(GUI.Button(new Rect(baseRect, baseRect + (buttonHeight * 5) + (10 * 5), 225, 75), "エクスポート(推奨)"))
            {
                extractAssetBundle();
            }

            //ExtractAssetBundleV2
            if (GUI.Button(new Rect(baseRect, baseRect + (buttonHeight * 6) + (10 * 6), 225, 75), "エクスポートV2(非推奨)"))
            {
                extractAssetBundleV2();
            }

            //Orientation2
            var allowOrientation2Str = "";
            if (Screen.orientation == defaultOrient)
            {
                allowOrientation2Str = "横画面にする(不安定)";
            }
            else
            {
                allowOrientation2Str = "縦画面にする";
            }
            if (GUI.Button(new Rect(baseRect + 10 + 225, baseRect, 225, 75), allowOrientation2Str))
            {
                if(Screen.orientation == defaultOrient)
                {
                    Screen.orientation = ScreenOrientation.Landscape;
                } else
                {
                    Screen.orientation = defaultOrient;
                }

            }
        }

        Texture2D GetTexture2D(FileSystemAB fileSystemAB ,string fileName)
        {
            fileName = fileName.ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName) + ".png";
            FileSystemAB.AssetData assetData;
            if (fileSystemAB.m_fileDatas.TryGetValue(fileName, out assetData))
            {
                AssetBundle assetBundle = assetData.assetBundle;
                return assetData.assetBundle.LoadAsset<Texture2D>(fileName);
            }
            return null;
        }

        void extractAssetBundle()
        {
            string bundleDirPath = Application.persistentDataPath + "/extract/";
            string[] bundleFiles = Directory.GetFiles(bundleDirPath);

            foreach(string bundle in bundleFiles)
            {
                string outputPath = bundleDirPath + "_" + Path.GetFileNameWithoutExtension(bundle);
                AssetBundle assetBundle = AssetBundle.LoadFromFile(bundle);
                if(assetBundle != null)
                {
                    Debug.Log("[CAST] Assetsエクスポート: " + bundle);
                    var assets = assetBundle.GetAllAssetNames();
                    foreach (var asset in assets)
                    {
                        var temp = outputPath + "/" + asset.Replace(".bytes", "");
                        createFile(temp);
                        var assetFile = assetBundle.LoadAsset(asset);
                        if(assetFile.GetType().ToString().IndexOf("TextAsset") != -1)
                        {
                            var tempAsset = assetFile as TextAsset;
                            FileStream fileStream = new FileStream(temp, FileMode.Create);
                            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                            binaryWriter.Write(tempAsset.bytes);
                            binaryWriter.Close();
                            fileStream.Close();
                        } else if (assetFile.GetType().ToString().IndexOf("Texture2D") != -1)
                        {
                            Texture2D tempAsset = ConvertReadTexs2D(assetFile as Texture2D);
                            var texure = tempAsset.EncodeToPNG();
                            FileStream fileStream = new FileStream(temp, FileMode.Create);
                            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                            binaryWriter.Write(texure);
                            binaryWriter.Close();
                            fileStream.Close();
                        } else
                        {
                            Debug.Log("[CAST]現在 " + assetFile.GetType().ToString() + " のエクスポートには対応していません");
                        }
                        Debug.Log(asset);
                    }
                }
                assetBundle.Unload(true);
                Debug.Log("[CAST] Assetsエクスポート終了！: " + Path.GetFileName(bundle));
            }
        }

        static void extractAssetBundleV2()
        {
            string bundleDirPath = Application.persistentDataPath + "/extract/";
            string[] bundleFiles = Directory.GetFiles(bundleDirPath);

            foreach (string bundle in bundleFiles)
            {
                string outputPath = bundleDirPath + "_" + Path.GetFileNameWithoutExtension(bundle);
                createFile(outputPath + "/" + "aaa");
                AssetBundle assetBundle = AssetBundle.LoadFromFile(bundle);
                if (assetBundle != null)
                {
                    var textAssets = assetBundle.LoadAllAssets<TextAsset>();
                    var textureAssets = assetBundle.LoadAllAssets<Texture2D>();

                    foreach(var asset in textAssets)
                    {
                        try
                        {
                            if(asset.name.IndexOf(".") != -1)
                            {
                                var filePath = outputPath + "/" + asset.name;
                                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                                BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                                binaryWriter.Write(asset.bytes);
                                binaryWriter.Close();
                                fileStream.Close();
                                Debug.Log("TextAssetEnd");
                            } else
                            {
                                Debug.Log("ファイルの拡張子がないためエクスポートできませんでした: " + asset.name);
                            }
                        } catch(UnauthorizedAccessException e)
                        {
                            Debug.Log("エラーが発生しました: " + asset.name);
                        }
                    }

                    foreach(var asset in textureAssets)
                    {
                        var filePath = outputPath + "/" + asset.name + ".png";
                        Texture2D tempAsset = ConvertReadTexs2D(asset);
                        var texure = tempAsset.EncodeToPNG();
                        FileStream fileStream = new FileStream(filePath, FileMode.Create);
                        BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                        binaryWriter.Write(texure);
                        binaryWriter.Close();
                        fileStream.Close();
                        Debug.Log("TextureAssetsEnd");
                    }
                }
                assetBundle.Unload(true);
                Debug.Log("[CAST] Assetsエクスポート終了！: " + Path.GetFileName(bundle));
            }
        }

        public static void createFile(string path)
        {
            var list = path.Split('/');
            string subPath = "";
            int i = 0;
            foreach(var value in list)
            {
                i++;
                subPath += "/" + value;
                if(i == list.Count())
                {
                    if(!File.Exists(subPath))
                    {
                    }
                } else
                {
                    if(!Directory.Exists(subPath))
                    {
                        Directory.CreateDirectory(subPath);
                    }
                }
            }
        }

        public static Texture2D ConvertReadTexs2D(Texture2D texture2D)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                    texture2D.width,
                    texture2D.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

            Graphics.Blit(texture2D, tmp);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(texture2D.width, texture2D.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D;
        }
    }
}
