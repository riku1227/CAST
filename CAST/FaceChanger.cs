using SCENE_EDIT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

/*
 * Warning ultra bad source code
 */

namespace CAST
{
    public class FaceChanger : MonoBehaviour
    {
        private Vector2 scrollViewVector = Vector2.zero;
        private int page = 0;
        private int blendPage = 0;
        private int posePage = 0;
        private int baseX = Screen.width - 310;
        private int baseY = 10;
        private GUIStyle buttonStyle = null;

        private string[][] faces = new string[23][];
        private string[][] faceBlends = new string[8][];

        private string nowPoseName = "";

        private bool isLandscape = false;

        public static GameObject Init()
        {
            var go = new GameObject();
            DontDestroyOnLoad(go);
            go.AddComponent<FaceChanger>();
            return go;
        }

        public static void setShow(bool b)
        {
            App.Config.showFaceChanger = b;
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.touches[0];
                if (touch.phase == TouchPhase.Moved)
                {
                    float x = touch.position.x;
                    float y = touch.position.y;
                    if(x >= Screen.width -310 && x <= Screen.width -10 && y >= Screen.height - 1000 && y <= Screen.height - 10)
                    {
                        scrollViewVector.y += touch.deltaPosition.y;
                    }
                }
            }
        }

        private void Awake()
        {
            InitFace();
            if (Screen.width > Screen.height)
            {
                isLandscape = true;
            }
            else
            {
                isLandscape = false;
            }
        }

        void OnGUI()
        {
            buttonStyle = GUI.skin.GetStyle("button");
            buttonStyle.fontSize = 22;

            if(App.Config.showExPoseSelect == 2)
            {
                var poseWidth = Screen.width / 8;
                List<EditPoseData> poseData = EditPoseData.m_dataList;
                if(posePage > 0)
                {
                    if(GUI.Button(new Rect(Screen.width - (poseWidth * 3), Screen.height - (poseWidth * 8.5F), poseWidth, poseWidth), "←"))
                    {
                        posePage--;
                    }
                }

                if(posePage < (poseData.Count / 40))
                {
                    if (GUI.Button(new Rect(Screen.width - (poseWidth * 3) + (poseWidth * 1.2F + 10), Screen.height - (poseWidth * 8.5F), poseWidth , poseWidth ), "→"))
                    {
                        posePage++;
                    }
                }

                GUI.Box(new Rect(0, (Screen.height - (poseWidth * 6) - poseWidth / 2), Screen.width, (poseWidth * 5)), "");
                GUI.Box(new Rect(10, (Screen.height - (poseWidth * 6) - poseWidth / 2) - 50 - 100, 450, 100), "");
                GUIStyle labelStyle = GUI.skin.label;
                labelStyle.fontSize = 45;
                GUI.Label(new Rect(10, (Screen.height - (poseWidth * 6) - poseWidth / 2) - 50 - 100, 450, 100), nowPoseName, labelStyle);

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (j + (i * 8) + (posePage * 40) < poseData.Count)
                        {
                            Color color = GUI.color;
                            color.a = 0.8F;
                            GUI.color = color;
                            GUI.DrawTexture(new Rect(j * poseWidth + 5, i * poseWidth + (Screen.height - (poseWidth * 6) - poseWidth / 2) + 5, poseWidth - 10, poseWidth - 10), SceneEdit.Instance.TextureLoader.LoadTexture(poseData[j + (i * 8) + (posePage * 40)].IconFileName));
                            color.a = 1F;
                            GUI.color = color;
                            if (GUI.Button(new Rect(j * poseWidth + 5, i * poseWidth + (Screen.height - (poseWidth * 6) - poseWidth / 2) + 5, poseWidth - 10, poseWidth - 10), "", new GUIStyle()))
                            {
                                poseData[j + (i * 8) + (posePage * 40)].PoseApply();
                                nowPoseName = poseData[j + (i * 8) + (posePage * 40)].FileName;
                            }
                        }
                    }
                }
            }

            if (App.Config.showFaceChanger)
            {
                if(App.Config.showExPoseSelect != 2)
                {
                    if(isLandscape)
                    {
                        if (GUI.Button(new Rect(100, 10, 280, 80), "拡張ポーズ", buttonStyle))
                        {
                            App.Config.showExPoseSelect = 1;
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(baseX, baseY + 1100, 280, 80), "拡張ポーズ", buttonStyle))
                        {
                            App.Config.showExPoseSelect = 1;
                        }
                    }
                }
                

                if (App.Config.oldFaceChangerUI)
                {
                    oldUI();
                }
                else
                {

                    if (page > 0)
                    {
                        if(GUI.Button(new Rect(baseX, baseY + 885, 140, 80), "←", buttonStyle))
                        {
                            page -= 1;
                        }
                    }
                    if(page < 22)
                    {
                        if (GUI.Button(new Rect(baseX + 140, baseY + 885, 140, 80), "→", buttonStyle))
                        {
                            page += 1;
                        }
                    }

                    for(int i = 0; i < faces[page].Length; i++)
                    {
                        if (GUI.Button(new Rect(baseX, baseY + (125 * i), 280, 125), faces[page][i], buttonStyle))
                        {
                            SceneEdit.Instance.EditMaid.FaceAnime(faces[page][i], 1f, 0);
                        }
                    }

                    
                    if (blendPage > 0)
                    {
                        if (GUI.Button(new Rect(10, baseY + 885, 140, 80), "←", buttonStyle))
                        {
                            blendPage -= 1;
                        }
                    }
                    if (blendPage < 7)
                    {
                        if (GUI.Button(new Rect(10 + 140, baseY + 885, 140, 80), "→", buttonStyle))
                        {
                            blendPage += 1;
                        }
                    }

                    for (int i = 0; i < faceBlends[blendPage].Length; i++)
                    {
                        if (GUI.Button(new Rect(10, baseY + (125 * i) + 250, 280, 125), faceBlends[blendPage][i], buttonStyle))
                        {
                            SceneEdit.Instance.EditMaid.FaceBlend(faceBlends[blendPage][i]);
                        }
                    }
                }
            }
        }

        void InitFace()
        {
            faces[0] = new string[] { "通常", "オリジナル", "微笑み", "笑顔", "にっこり", "優しさ", "発情" };
            faces[1] = new string[] { "ジト目", "閉じ目", "思案伏せ目", "ドヤ顔", "引きつり笑顔", "苦笑い", "困った" };
            faces[2] = new string[] { "疑問", "ぷんすか", "むー", "泣き", "拗ね", "照れ", "悲しみ２" };
            faces[3] = new string[] { "きょとん", "びっくり", "少し怒り", "怒り", "照れ叫び", "誘惑", "接吻" };
            faces[4] = new string[] { "居眠り安眠", "まぶたギュ", "目を見開いて", "痛みで目を見開いて", "恥ずかしい", "ためいき", "がっかり" };
            faces[5] = new string[] { "口開け", "目口閉じ", "ウインク照れ", "にっこり照れ", "ダンス目つむり", "ダンスあくび", "ダンスびっくり" };
            faces[6] = new string[] { "ダンス微笑み", "ダンス目あけ", "ダンス目とじ", "ダンス誘惑", "ダンス困り顔", "ダンスウインク", "ダンス真剣" };
            faces[7] = new string[] { "ダンス真剣２", "ダンス憂い", "ダンスジト目", "ダンスキス", "エロ期待", "エロ緊張", "エロ怯え" };
            faces[8] = new string[] { "エロ痛み我慢", "エロ痛み我慢２", "エロ痛み我慢３", "絶頂射精後１", "絶頂射精後２", "興奮射精後１", "興奮射精後２" };
            faces[9] = new string[] { "通常射精後１", "通常射精後２", "余韻弱", "追加よだれ", "エロメソ泣き", "エロ絶頂", "エロ放心" };
            faces[10] = new string[] { "エロ舌責", "エロ舌責嫌悪", "エロ舌責快楽", "エロ興奮０", "エロ興奮１", "エロ興奮２", "エロ興奮３" };
            faces[11] = new string[] { "エロ嫌悪１", "エロ通常１", "エロ通常２", "エロ通常３", "エロ好感１", "エロ好感２", "エロ好感３" };
            faces[12] = new string[] { "エロ我慢１", "エロ我慢２", "エロ我慢３", "エロ痛み１", "エロ痛み２", "エロ痛み３", "エロ羞恥１" };
            faces[13] = new string[] { "エロ羞恥２", "エロ羞恥３", "あーん", "エロ舐め嫌悪", "エロ舐め嫌悪２", "エロ舐め快楽", "エロ舐め快楽２" };
            faces[14] = new string[] { "エロ舐め愛情", "エロ舐め愛情２", "エロ舐め通常", "エロ舐め通常２", "エロフェラ快楽", "エロフェラ愛情", "エロフェラ嫌悪" };
            faces[15] = new string[] { "エロフェラ通常", "閉じ舐め嫌悪", "閉じ舐め嫌悪２", "閉じ舐め快楽", "閉じ舐め快楽２", "閉じ舐め愛情", "閉じ舐め愛情２" };
            faces[16] = new string[] { "閉じ舐め通常", "閉じ舐め通常２", "閉じフェラ快楽", "閉じフェラ愛情", "閉じフェラ嫌悪", "閉じフェラ通常", "頬０涙１" };
            faces[17] = new string[] { "頬０涙２", "頬０涙３", "頬１涙０", "頬１涙１", "頬１涙２", "頬１涙３", "頬２涙０" };
            faces[18] = new string[] { "頬２涙１", "頬２涙２", "頬２涙３", "頬３涙０", "頬３涙１", "頬３涙２", "頬３涙３" };
            faces[19] = new string[] { "頬０涙０よだれ", "頬０涙１よだれ", "頬０涙２よだれ", "頬０涙３よだれ", "頬１涙０よだれ", "頬１涙１よだれ", "頬１涙２よだれ" };
            faces[20] = new string[] { "頬１涙３よだれ", "頬２涙０よだれ", "頬２涙１よだれ", "頬２涙２よだれ", "頬２涙３よだれ", "頬３涙０よだれ", "頬３涙１よだれ" };
            faces[21] = new string[] { "頬３涙２よだれ", "頬３涙３よだれ", "デフォ", "エラー", "n", "a", "i" };
            faces[22] = new string[] { "涙0", "涙1", "涙2", "涙3", "涙4" };

            faceBlends[0] = new string[] { "頬０涙１", "頬０涙２", "頬０涙３", "頬１涙０", "頬１涙１" };
            faceBlends[1] = new string[] { "頬１涙２", "頬１涙３", "頬２涙０", "頬２涙１", "頬２涙２" };
            faceBlends[2] = new string[] { "頬２涙３", "頬３涙０", "頬３涙１", "頬３涙２", "頬３涙３" };
            faceBlends[3] = new string[] { "頬０涙０よだれ", "頬０涙１よだれ", "頬０涙２よだれ", "頬０涙３よだれ", "頬１涙０よだれ" };
            faceBlends[4] = new string[] { "頬１涙１よだれ", "頬１涙２よだれ", "頬１涙３よだれ", "頬２涙０よだれ", "頬２涙１よだれ" };
            faceBlends[5] = new string[] { "頬２涙２よだれ", "頬２涙３よだれ", "頬３涙０よだれ", "頬３涙１よだれ", "頬３涙３よだれ" };
            faceBlends[6] = new string[] { "デフォ", "エラー", "n", "a", "i" };
            faceBlends[7] = new string[] { "涙0", "涙1", "涙2", "涙3", "涙4" };
        }

        void oldUI()
        {
            scrollViewVector = GUI.BeginScrollView(new Rect(Screen.width - 310, 10, 300, 1000), scrollViewVector, new Rect(0, 0, 280, 19875));

            if (GUI.Button(new Rect(0, 0, 280, 125), "通常", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("通常", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 125, 280, 125), "オリジナル", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("オリジナル", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 250, 280, 125), "微笑み", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("微笑み", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 375, 280, 125), "笑顔", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("笑顔", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 500, 280, 125), "にっこり", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("にっこり", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 625, 280, 125), "優しさ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("優しさ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 750, 280, 125), "発情", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("発情", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 875, 280, 125), "ジト目", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ジト目", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 1000, 280, 125), "閉じ目", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ目", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 1125, 280, 125), "思案伏せ目", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("思案伏せ目", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 1250, 280, 125), "ドヤ顔", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ドヤ顔", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 1375, 280, 125), "引きつり笑顔", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("引きつり笑顔", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 1500, 280, 125), "苦笑い", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("苦笑い", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 1625, 280, 125), "困った", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("困った", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 1750, 280, 125), "疑問", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("疑問", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 1875, 280, 125), "ぷんすか", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ぷんすか", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 2000, 280, 125), "むー", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("むー", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 2125, 280, 125), "泣き", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("泣き", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 2250, 280, 125), "拗ね", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("拗ね", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 2375, 280, 125), "照れ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("照れ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 2500, 280, 125), "悲しみ２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("悲しみ２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 2625, 280, 125), "きょとん", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("きょとん", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 2750, 280, 125), "びっくり", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("びっくり", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 2875, 280, 125), "少し怒り", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("少し怒り", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 3000, 280, 125), "怒り", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("怒り", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 3125, 280, 125), "照れ叫び", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("照れ叫び", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 3250, 280, 125), "誘惑", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("誘惑", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 3375, 280, 125), "接吻", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("接吻", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 3500, 280, 125), "居眠り安眠", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("居眠り安眠", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 3625, 280, 125), "まぶたギュ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("まぶたギュ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 3750, 280, 125), "目を見開いて", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("目を見開いて", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 3875, 280, 125), "痛みで目を見開いて", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("痛みで目を見開いて", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 4000, 280, 125), "恥ずかしい", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("恥ずかしい", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 4125, 280, 125), "ためいき", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ためいき", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 4250, 280, 125), "がっかり", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("がっかり", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 4375, 280, 125), "口開け", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("口開け", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 4500, 280, 125), "目口閉じ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("目口閉じ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 4625, 280, 125), "ウインク照れ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ウインク照れ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 4750, 280, 125), "にっこり照れ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("にっこり照れ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 4875, 280, 125), "ダンス目つむり", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス目つむり", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 5000, 280, 125), "ダンスあくび", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンスあくび", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 5125, 280, 125), "ダンスびっくり", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンスびっくり", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 5250, 280, 125), "ダンス微笑み", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス微笑み", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 5375, 280, 125), "ダンス目あけ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス目あけ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 5500, 280, 125), "ダンス目とじ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス目とじ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 5625, 280, 125), "ダンス誘惑", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス誘惑", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 5750, 280, 125), "ダンス困り顔", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス困り顔", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 5875, 280, 125), "ダンスウインク", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンスウインク", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 6000, 280, 125), "ダンス真剣", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス真剣", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 6125, 280, 125), "ダンス真剣２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス真剣２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 6250, 280, 125), "ダンス憂い", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンス憂い", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 6375, 280, 125), "ダンスジト目", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンスジト目", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 6500, 280, 125), "ダンスキス", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("ダンスキス", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 6625, 280, 125), "エロ期待", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ期待", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 6750, 280, 125), "エロ緊張", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ緊張", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 6875, 280, 125), "エロ怯え", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ怯え", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 7000, 280, 125), "エロ痛み我慢", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ痛み我慢", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 7125, 280, 125), "エロ痛み我慢２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ痛み我慢２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 7250, 280, 125), "エロ痛み我慢３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ痛み我慢３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 7375, 280, 125), "絶頂射精後１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("絶頂射精後１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 7500, 280, 125), "絶頂射精後２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("絶頂射精後２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 7625, 280, 125), "興奮射精後１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("興奮射精後１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 7750, 280, 125), "興奮射精後２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("興奮射精後２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 7875, 280, 125), "通常射精後１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("通常射精後１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 8000, 280, 125), "通常射精後２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("通常射精後２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 8125, 280, 125), "余韻弱", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("余韻弱", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 8250, 280, 125), "追加よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("追加よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 8375, 280, 125), "エロメソ泣き", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロメソ泣き", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 8500, 280, 125), "エロ絶頂", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ絶頂", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 8625, 280, 125), "エロ放心", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ放心", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 8750, 280, 125), "エロ舌責", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舌責", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 8875, 280, 125), "エロ舌責嫌悪", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舌責嫌悪", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 9000, 280, 125), "エロ舌責快楽", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舌責快楽", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 9125, 280, 125), "エロ興奮０", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ興奮０", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 9250, 280, 125), "エロ興奮１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ興奮１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 9375, 280, 125), "エロ興奮２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ興奮２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 9500, 280, 125), "エロ興奮３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ興奮３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 9625, 280, 125), "エロ嫌悪１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ嫌悪１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 9750, 280, 125), "エロ通常１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ通常１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 9875, 280, 125), "エロ通常２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ通常２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 10000, 280, 125), "エロ通常３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ通常３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 10125, 280, 125), "エロ好感１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ好感１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 10250, 280, 125), "エロ好感２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ好感２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 10375, 280, 125), "エロ好感３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ好感３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 10500, 280, 125), "エロ我慢１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ我慢１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 10625, 280, 125), "エロ我慢２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ我慢２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 10750, 280, 125), "エロ我慢３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ我慢３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 10875, 280, 125), "エロ痛み１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ痛み１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 11000, 280, 125), "エロ痛み２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ痛み２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 11125, 280, 125), "エロ痛み３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ痛み３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 11250, 280, 125), "エロ羞恥１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ羞恥１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 11375, 280, 125), "エロ羞恥２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ羞恥２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 11500, 280, 125), "エロ羞恥３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ羞恥３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 11625, 280, 125), "あーん", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("あーん", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 11750, 280, 125), "エロ舐め嫌悪", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舐め嫌悪", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 11875, 280, 125), "エロ舐め嫌悪２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舐め嫌悪２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 12000, 280, 125), "エロ舐め快楽", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舐め快楽", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 12125, 280, 125), "エロ舐め快楽２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舐め快楽２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 12250, 280, 125), "エロ舐め愛情", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舐め愛情", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 12375, 280, 125), "エロ舐め愛情２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舐め愛情２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 12500, 280, 125), "エロ舐め通常", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舐め通常", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 12625, 280, 125), "エロ舐め通常２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロ舐め通常２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 12750, 280, 125), "エロフェラ快楽", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロフェラ快楽", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 12875, 280, 125), "エロフェラ愛情", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロフェラ愛情", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 13000, 280, 125), "エロフェラ嫌悪", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロフェラ嫌悪", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 13125, 280, 125), "エロフェラ通常", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エロフェラ通常", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 13250, 280, 125), "閉じ舐め嫌悪", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ舐め嫌悪", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 13375, 280, 125), "閉じ舐め嫌悪２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ舐め嫌悪２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 13500, 280, 125), "閉じ舐め快楽", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ舐め快楽", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 13625, 280, 125), "閉じ舐め快楽２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ舐め快楽２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 13750, 280, 125), "閉じ舐め愛情", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ舐め愛情", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 13875, 280, 125), "閉じ舐め愛情２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ舐め愛情２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 14000, 280, 125), "閉じ舐め通常", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ舐め通常", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 14125, 280, 125), "閉じ舐め通常２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じ舐め通常２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 14250, 280, 125), "閉じフェラ快楽", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じフェラ快楽", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 14375, 280, 125), "閉じフェラ愛情", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じフェラ愛情", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 14500, 280, 125), "閉じフェラ嫌悪", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じフェラ嫌悪", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 14625, 280, 125), "閉じフェラ通常", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("閉じフェラ通常", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 14750, 280, 125), "頬０涙１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬０涙１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 14875, 280, 125), "頬０涙２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬０涙２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 15000, 280, 125), "頬０涙３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬０涙３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 15125, 280, 125), "頬１涙０", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬１涙０", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 15250, 280, 125), "頬１涙１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬１涙１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 15375, 280, 125), "頬１涙２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬１涙２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 15500, 280, 125), "頬１涙３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬１涙３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 15625, 280, 125), "頬２涙０", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬２涙０", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 15750, 280, 125), "頬２涙１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬２涙１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 15875, 280, 125), "頬２涙２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬２涙２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 16000, 280, 125), "頬２涙３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬２涙３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 16125, 280, 125), "頬３涙０", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬３涙０", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 16250, 280, 125), "頬３涙１", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬３涙１", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 16375, 280, 125), "頬３涙２", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬３涙２", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 16500, 280, 125), "頬３涙３", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬３涙３", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 16625, 280, 125), "頬０涙０よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬０涙０よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 16750, 280, 125), "頬０涙１よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬０涙１よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 16875, 280, 125), "頬０涙２よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬０涙２よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 17000, 280, 125), "頬０涙３よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬０涙３よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 17125, 280, 125), "頬１涙０よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬１涙０よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 17250, 280, 125), "頬１涙１よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬１涙１よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 17375, 280, 125), "頬１涙２よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬１涙２よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 17500, 280, 125), "頬１涙３よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬１涙３よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 17625, 280, 125), "頬２涙０よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬２涙０よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 17750, 280, 125), "頬２涙１よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬２涙１よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 17875, 280, 125), "頬２涙２よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬２涙２よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 18000, 280, 125), "頬２涙３よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬２涙３よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 18125, 280, 125), "頬３涙０よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬３涙０よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 18250, 280, 125), "頬３涙１よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬３涙１よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 18375, 280, 125), "頬３涙２よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬３涙２よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 18500, 280, 125), "頬３涙３よだれ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("頬３涙３よだれ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 18625, 280, 125), "デフォ", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("デフォ", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 18750, 280, 125), "エラー", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("エラー", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 18875, 280, 125), "n", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("n", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 19000, 280, 125), "a", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("a", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 19125, 280, 125), "i", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("i", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 19250, 280, 125), "涙0", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("涙0", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 19375, 280, 125), "涙1", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("涙1", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 19500, 280, 125), "涙2", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("涙2", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 19625, 280, 125), "涙3", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("涙3", 1f, 0);
            }
            if (GUI.Button(new Rect(0, 19750, 280, 125), "涙4", buttonStyle))
            {
                SceneEdit.Instance.EditMaid.FaceAnime("涙4", 1f, 0);
            }
            GUI.EndScrollView();
        }
    }
}
