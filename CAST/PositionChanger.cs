using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CAST
{
    public class PositionChanger : MonoBehaviour
    {
        CameraMain cameraMain = null;
        Maid editMaid;
        float changeValue = 0.05F;
        float rotationValue = 5F;
        float cameraValue = 0.05F;

        float screenBaseWidth = 0F;
        float screenBaseHeight = 0F;

        float maidPositionZ = 0F;

        bool isLandscape = false;

        public static GameObject Init()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<PositionChanger>();
            return go;
        }

        private void Awake()
        {
            cameraMain = GameMain.Instance.MainCamera;

        }

        void OnGUI()
        {
            if(App.Config.showPositionChanger)
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

                editMaid = SceneEdit.Instance.EditMaid;
                var buttonStyle = GUI.skin.GetStyle("button");
                buttonStyle.fontSize = 34;

                var editStyle = GUI.skin.GetStyle("textField");
                editStyle.fontSize = 40;

                if(isLandscape)
                {
                    GUI.Window(57859868, new Rect(10, screenBaseHeight * 30, screenBaseWidth * 38, screenBaseHeight * 20), onMiadPositionWindow, "メイドポジション");
                    GUI.Window(2137654285, new Rect(screenBaseWidth * 38 + 20, screenBaseHeight * 30, screenBaseWidth * 38, screenBaseHeight * 20), onMiadRotationWindow, "メイドローテーション");
                }
                else
                {
                    GUI.Window(57859868, new Rect(10, screenBaseHeight * 50, screenBaseWidth * 38, screenBaseHeight * 20), onMiadPositionWindow, "メイドポジション");
                    GUI.Window(2137654285, new Rect(10, screenBaseHeight * 70 + 20, screenBaseWidth * 38, screenBaseHeight * 20), onMiadRotationWindow, "メイドローテーション");
                }
                //GUI.Window(633238941, new Rect(Screen.width - screenBaseWidth * 38 - 10, screenBaseHeight * 50 + 20, screenBaseWidth * 38, screenBaseHeight * 20), onCameraPositionWindow, "カメラポジション");

                //Camera
                var cameraHeightPadding = 0F;
                if (isLandscape)
                {
                    cameraHeightPadding = 190;
                }
                else
                {
                    cameraHeightPadding = 1000F;
                }
                cameraValue = float.Parse(GUI.TextField(new Rect(Screen.width - 280 - 10, Screen.height - cameraHeightPadding - 65, 280, 60), cameraValue.ToString(), editStyle));

                if (GUI.Button(new Rect(Screen.width - 280 - 10, Screen.height - cameraHeightPadding, 90, 90), "▲", buttonStyle))
                {
                    Vector3 vector3 = cameraMain.GetPos();
                    vector3.z += cameraValue;
                    setCameraPos(vector3);
                }

                if (GUI.Button(new Rect(Screen.width - 280 + 105 - 20, Screen.height - cameraHeightPadding, 90, 90), "↑", buttonStyle))
                {
                    Vector3 vector3 = cameraMain.GetPos();
                    vector3.y += cameraValue;
                    setCameraPos(vector3);
                }

                if (GUI.Button(new Rect(Screen.width - 280 + 200 - 20, Screen.height - cameraHeightPadding, 90, 90), "▼", buttonStyle))
                {
                    Vector3 vector3 = cameraMain.GetPos();
                    vector3.z -= cameraValue;
                    setCameraPos(vector3);
                }

                if (GUI.Button(new Rect(Screen.width - 280 - 10, Screen.height - cameraHeightPadding + 95, 90, 90), "←", buttonStyle))
                {
                    Vector3 vector3 = cameraMain.GetPos();
                    vector3.x += cameraValue;
                    setCameraPos(vector3);
                }

                if (GUI.Button(new Rect(Screen.width - 280 + 105 - 20, Screen.height - cameraHeightPadding + 95, 90, 90), "↓", buttonStyle))
                {
                    Vector3 vector3 = cameraMain.GetPos();
                    vector3.y -= cameraValue;
                    setCameraPos(vector3);
                }

                if (GUI.Button(new Rect(Screen.width - 280 + 200 - 20, Screen.height - cameraHeightPadding + 95, 90, 90), "→", buttonStyle))
                {
                    Vector3 vector3 = cameraMain.GetPos();
                    vector3.x -= cameraValue;
                    setCameraPos(vector3);
                }
            }
        }

        private void onMiadPositionWindow(int id)
        {
            var editStyle = GUI.skin.GetStyle("textField");
            editStyle.fontSize = 40;
            editStyle.alignment = TextAnchor.MiddleCenter;
            var windowWBase = screenBaseWidth * 38 / 100;
            var windowHBase = screenBaseHeight * 20 / 100;
            Vector3 vector3 = editMaid.GetPos();

            changeValue = float.Parse(GUI.TextField(new Rect(10, 30, windowWBase * 100 - 20, windowHBase * 15), changeValue.ToString(), editStyle));
            if (GUI.Button(new Rect(10, 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "▲"))
            {
                vector3.z += changeValue;
            }
            vector3.z = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (40 + windowHBase * 15), windowWBase * 50, windowHBase * 20), vector3.z.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "▼"))
            {
                vector3.z -= changeValue;
            }

            if (GUI.Button(new Rect(10, 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "↑"))
            {
                vector3.y += changeValue;
            }
            vector3.y = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (50 + windowHBase * 15 + (windowHBase * 20)), windowWBase * 50, windowHBase * 20), vector3.y.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "↓"))
            {
                vector3.y -= changeValue;
            }

            if (GUI.Button(new Rect(10, 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 20, windowHBase * 20), "←"))
            {
                vector3.x += changeValue;
            }
            vector3.x = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (60 + windowHBase * 15 + (windowHBase * 20 * 2)), windowWBase * 50, windowHBase * 20), vector3.x.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 20, windowHBase * 20), "→"))
            {
                vector3.x -= changeValue;
            }
            editMaid.SetPos(vector3);
        }

        private void onMiadRotationWindow(int id)
        {
            var editStyle = GUI.skin.GetStyle("textField");
            editStyle.fontSize = 40;
            editStyle.alignment = TextAnchor.MiddleCenter;
            var windowWBase = screenBaseWidth * 38 / 100;
            var windowHBase = screenBaseHeight * 20 / 100;
            Vector3 vector3 = editMaid.GetRot();

            rotationValue = float.Parse(GUI.TextField(new Rect(10, 30, windowWBase * 100 - 20, windowHBase * 15), rotationValue.ToString(), editStyle));
            if (GUI.Button(new Rect(10, 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "○"))
            {
                vector3.y += rotationValue;
            }
            vector3.y = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (40 + windowHBase * 15), windowWBase * 50, windowHBase * 20), vector3.y.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "◎"))
            {
                vector3.y -= rotationValue;
            }

            if (GUI.Button(new Rect(10, 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "↑"))
            {
                vector3.x += rotationValue;
            }
            vector3.x = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (50 + windowHBase * 15 + (windowHBase * 20)), windowWBase * 50, windowHBase * 20), vector3.x.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "↓"))
            {
                vector3.x -= rotationValue;
            }

            if (GUI.Button(new Rect(10, 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 20, windowHBase * 20), "←"))
            {
                vector3.z += rotationValue;
            }
            vector3.z = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (60 + windowHBase * 15 + (windowHBase * 20 * 2)), windowWBase * 50, windowHBase * 20), vector3.z.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 20, windowHBase * 20), "→"))
            {
                vector3.z -= rotationValue;
            }
            editMaid.SetRot(vector3);
        }

        /* private void onCameraPositionWindow(int id)
        {
            var editStyle = GUI.skin.GetStyle("textField");
            editStyle.fontSize = 40;
            editStyle.alignment = TextAnchor.MiddleCenter;
            var windowWBase = screenBaseWidth * 38 / 100;
            var windowHBase = screenBaseHeight * 20 / 100;
            Vector3 vector3 = cameraMain.GetPos();

            cameraValue = float.Parse(GUI.TextField(new Rect(10, 30, windowWBase * 100 - 20, windowHBase * 15), cameraValue.ToString(), editStyle));
            if (GUI.Button(new Rect(10, 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "▲"))
            {
                vector3.z += cameraValue;
            }
            vector3.z = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (40 + windowHBase * 15), windowWBase * 50, windowHBase * 20), vector3.z.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 40 + windowHBase * 15, windowWBase * 20, windowHBase * 20), "▼"))
            {
                vector3.z -= cameraValue;
            }

            if (GUI.Button(new Rect(10, 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "↑"))
            {
                vector3.y += cameraValue;
            }
            vector3.y = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (50 + windowHBase * 15 + (windowHBase * 20)), windowWBase * 50, windowHBase * 20), vector3.y.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 50 + windowHBase * 15 + (windowHBase * 20), windowWBase * 20, windowHBase * 20), "↓"))
            {
                vector3.y -= cameraValue;
            }

            if (GUI.Button(new Rect(10, 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 20, windowHBase * 20), "←"))
            {
                vector3.x += cameraValue;
            }
            vector3.x = float.Parse(GUI.TextField(new Rect(((10 * 2) + windowWBase * 20), (60 + windowHBase * 15 + (windowHBase * 20 * 2)), windowWBase * 50, windowHBase * 20), vector3.x.ToString(), editStyle));
            if (GUI.Button(new Rect(((10 * 3) + windowWBase * 20 + windowWBase * 50), 60 + windowHBase * 15 + (windowHBase * 20 * 2), windowWBase * 20, windowHBase * 20), "→"))
            {
                vector3.x -= cameraValue;
            }
            setCameraPos(vector3);
        } */

        public static void setShow(bool b)
        {
            App.Config.showPositionChanger = b;
        }

        private void setCameraPos(Vector3 vector)
        {
            cameraMain.SetPos(vector);
            cameraMain.SetTargetPos(vector, true);
            cameraMain.SetDistance(0f, true);
        }
    }
}
