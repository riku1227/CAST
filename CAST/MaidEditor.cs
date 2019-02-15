using CAST.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CAST
{
    public class MaidEditor : MonoBehaviour
    {
        float screenBaseWidth = 0F;
        float screenBaseHeight = 0F;

        bool showSpawnUI = false;
        bool showMaidDeleteWindow = false;
        bool showSavePresetWindow = false;
        bool showSaveWearPresetWindow = false;
        bool showLoadPresetWindow = false;
        bool showLoadWearPresetWindow = false;
        bool isLandscape = false;
        bool isShowUI = true;

        int spawnPage = 0;
        int presetPage = 0;
        int WpresetPage = 0;

        Maid[] maidList = new Maid[18];
        int maidCount = 0;

        int nowMaid = 0;

        List<int> freeMaidSpace = new List<int>();

        List<CharacterMgr.MaidFileData> maidFileList = GameMain.Instance.CharacterMgr.MaidFileListLoad();

        string savePresetFileName = "FileName";
        string saveWearPresetFileName = "WearPresetFileName";
        string[] presetFiles;
        string[] presetFilesW;

        public static GameObject Init()
        {
            var go = new GameObject();
            //DontDestroyOnLoad(go);
            go.AddComponent<MaidEditor>();
            return go;
        }

        void Awake()
        {
            maidList[0] = SceneEdit.Instance.EditMaid;
            maidCount++;
        }

        void OnGUI()
        {
            if(App.Config.showMaidEditor)
            {
                if(Screen.width > Screen.height)
                {
                    screenBaseWidth = Screen.height / 100;
                    screenBaseHeight = Screen.width / 100;
                    isLandscape = true;
                } else
                {
                    screenBaseWidth = Screen.width / 100;
                    screenBaseHeight = Screen.height / 100;
                    isLandscape = false;
                }
                
                MenuUI();
                MaidSelectUI();

                if(showSpawnUI)
                {
                    SpawnUI();
                }

                if(showLoadPresetWindow)
                {
                    LoadPositionPresetUI();
                }

                if (showLoadWearPresetWindow)
                {
                    LoadWearPresetUI();
                }
            }
        }

        void MenuUI()
        {
            var buttonStyle = GUI.skin.GetStyle("button");
            buttonStyle.fontSize = 34;
            var baseRect = 22;
            float buttonWidth = screenBaseWidth * 30;
            float buttonHeight = screenBaseWidth * 10;

            float heightPadding = 0F;

            if(isLandscape)
            {
                heightPadding = (-screenBaseHeight) * 5;
            } else
            {
                heightPadding = 0;
            }

            if(GUI.Button(new Rect(10, screenBaseHeight * 10 + heightPadding, buttonWidth, buttonHeight), "召喚", buttonStyle))
            {
                showSpawnUI = !showSpawnUI;
                
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 15 + heightPadding, buttonWidth, buttonHeight), "位置変更", buttonStyle))
            {
                PositionChanger.setShow(!App.Config.showPositionChanger);
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 20 + heightPadding, buttonWidth, buttonHeight), "位置プリセットをロード", buttonStyle))
            {
                string presetDirectory = AFileSystemBase.base_path + App.base_path + App.position_preset_path;
                var allFile = Directory.GetFiles(presetDirectory);
                List<string> vs = new List<string>();
                foreach (var item in allFile)
                {
                    if (Path.GetExtension(item).IndexOf("json") != -1)
                    {
                        vs.Add(item);
                    }
                }
                presetFiles = vs.ToArray();
                showLoadPresetWindow = !showLoadPresetWindow;
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 25 + heightPadding, buttonWidth, buttonHeight), "衣服プリセットをロード", buttonStyle))
            {
                string presetDirectory = AFileSystemBase.base_path + App.base_path + App.wear_preset_path;
                var allFile = Directory.GetFiles(presetDirectory);
                List<string> vsw = new List<string>();
                foreach (var item in allFile)
                {
                    if (Path.GetExtension(item).IndexOf("json") != -1)
                    {
                        vsw.Add(item);
                    }
                }
                presetFilesW = vsw.ToArray();
                showLoadWearPresetWindow = !showLoadWearPresetWindow;
            }

            string maidStr = "";

            if (SceneEdit.Instance.EditMaid.boMabataki)
            {
                maidStr = "瞬きを無効化する";
            }
            else
            {
                maidStr = "瞬きを有効化する";
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 30 + heightPadding, buttonWidth, buttonHeight), maidStr, buttonStyle))
            {
                SceneEdit.Instance.EditMaid.boMabataki = !SceneEdit.Instance.EditMaid.boMabataki;
            }

            string uiShowStr = "";
            if (isShowUI)
            {
                uiShowStr = "UIを非表示にする";
            }
            else
            {
                uiShowStr = "UIを表示する";
            }
            if (GUI.Button(new Rect(10, screenBaseHeight * 35 + heightPadding, buttonWidth, buttonHeight), uiShowStr, buttonStyle))
            {
                if(isShowUI)
                {
                    SceneEdit.Instance.MainMenuUI.CloseFast();
                    isShowUI = false;
                } else
                {
                    SceneEdit.Instance.MainMenuUI.OpenFast();
                    isShowUI = true;
                }
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 40 + heightPadding, buttonWidth, buttonHeight), "モザ消しモデル化", buttonStyle))
            {
                List<string> fileNames = new List<string>();
                foreach (MPN value in Enum.GetValues(typeof(MPN)))
                {
                    MaidProp maidProp = SceneEdit.Instance.EditMaid.GetProp(value);
                    fileNames.Add(maidProp.strFileName);
                    if (value != MPN.body)
                    {
                        SceneEdit.Instance.EditMaid.DelProp(value);
                    }
                }

                SceneEdit.Instance.EditMaid.SetProp(MPN.body, "nomoza_body001_i_.menu", 0, false, false);
                int nowMPN = 0;
                foreach (MPN value in Enum.GetValues(typeof(MPN)))
                {
                    if (value != MPN.body)
                    {
                        if (value == MPN.head)
                        {
                            if (fileNames[nowMPN] != "face003_i_.menu")
                            {
                                SceneEdit.Instance.EditMaid.SetProp(value, "face003_i_.menu", 0);
                                SceneEdit.Instance.EditMaid.SetProp(value, fileNames[nowMPN], 0);
                            }
                            else
                            {
                                SceneEdit.Instance.EditMaid.SetProp(value, "face004_i_.menu", 0);
                                SceneEdit.Instance.EditMaid.SetProp(value, fileNames[nowMPN], 0);
                            }
                        }
                        else if (value == MPN.hairf)
                        {
                            if (fileNames[nowMPN] != "hair_f039_i_.menu")
                            {
                                SceneEdit.Instance.EditMaid.SetProp(value, "hair_f039_i_.menu", 0);
                                SceneEdit.Instance.EditMaid.SetProp(value, fileNames[nowMPN], 0);
                            }
                            else
                            {
                                SceneEdit.Instance.EditMaid.SetProp(value, "hair_f117_i_.menu", 0);
                                SceneEdit.Instance.EditMaid.SetProp(value, fileNames[nowMPN], 0);
                            }
                        }
                        else if (value == MPN.hairr)
                        {
                            if (fileNames[nowMPN] != "hair_twinr3_i_.menu")
                            {
                                SceneEdit.Instance.EditMaid.SetProp(value, "hair_twinr3_i_.menu", 0);
                                SceneEdit.Instance.EditMaid.SetProp(value, fileNames[nowMPN], 0);
                            }
                            else
                            {
                                SceneEdit.Instance.EditMaid.SetProp(value, "hair_twinr_i_.menu", 0);
                                SceneEdit.Instance.EditMaid.SetProp(value, fileNames[nowMPN], 0);
                            }
                        }
                        else
                        {
                            SceneEdit.Instance.EditMaid.SetProp(value, fileNames[nowMPN], 0);
                        }
                    }
                    nowMPN++;
                }
                SceneEdit.Instance.EditMaid.AllProcProp();
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 45 + heightPadding, buttonWidth, buttonHeight), "SSを取る", buttonStyle))
            {
                GameMain.Instance.CMSystem.ScreenShotSuperSize = CMSystem.SSSuperSizeType.X4;
                shotScreenShot();
            }

        }

        void MaidSelectUI()
        {
            var heightPadding = 0F;
            if(isLandscape)
            {
                heightPadding = 10;
            } else
            {
                heightPadding = screenBaseHeight * 10;
            }
            GUI.Window(12345, new Rect(Screen.width - (screenBaseWidth * 35) - 10, heightPadding, screenBaseWidth * 35, screenBaseWidth * 60), OnMaidSelectWindow, "エディットメイド切り替え");

            if (showMaidDeleteWindow)
            {
                if(isLandscape)
                {
                    GUI.Window(15684, new Rect(screenBaseWidth * 50, screenBaseHeight * 10, screenBaseWidth * 50, screenBaseHeight * 10), OnMaidDeleteWindow, "本当に削除しますか？");
                } else
                {
                    GUI.Window(15684, new Rect(screenBaseWidth * 30, screenBaseHeight * 50, screenBaseWidth * 50, screenBaseHeight * 10), OnMaidDeleteWindow, "本当に削除しますか？");
                }
            }

            if (showSavePresetWindow)
            {
                if (isLandscape)
                {
                    GUI.Window(6549865, new Rect(screenBaseWidth * 50, screenBaseHeight * 10, screenBaseWidth * 50, screenBaseHeight * 10), SavePresetWindow, "プリセットを保存しますか？");
                }
                else
                {
                    GUI.Window(6549865, new Rect(screenBaseWidth * 30, screenBaseHeight * 50, screenBaseWidth * 50, screenBaseHeight * 10), SavePresetWindow, "プリセットを保存しますか？");
                }
            }

            if (showSaveWearPresetWindow)
            {
                if (isLandscape)
                {
                    GUI.Window(6549868, new Rect(screenBaseWidth * 50, screenBaseHeight * 10, screenBaseWidth * 50, screenBaseHeight * 10), SaveWearPresetWindow, "衣服プリセットを保存しますか？");
                }
                else
                {
                    GUI.Window(6549868, new Rect(screenBaseWidth * 30, screenBaseHeight * 50, screenBaseWidth * 50, screenBaseHeight * 10), SaveWearPresetWindow, "衣服プリセットを保存しますか？");
                }
            }
        }

        private void OnMaidSelectWindow(int id)
        {
            var padding = 10F;
            var labelSkin = GUI.skin.GetStyle("label");
            labelSkin.fontSize = 55;
            var windowWidth = screenBaseWidth * 35;
            var windowHeight = screenBaseWidth * 50;

            Maid editMaid = SceneEdit.Instance.EditMaid;
            MaidStatus status = editMaid.Status;

            GUI.Label(new Rect(padding, padding * 1.5F, screenBaseWidth * 30, screenBaseHeight * 10), status.m_lastName + " " + status.m_firstName, labelSkin);

            if(GUI.Button(new Rect(padding, windowWidth / 4, windowWidth / 4, windowWidth / 4), "←"))
            { 
                if(nowMaid > 0)
                {
                    nowMaid--;
                    while(maidList[nowMaid] == null)
                    {
                        nowMaid--;
                    }
                    SceneEdit.Instance.m_editMaid = maidList[nowMaid];
                    SceneEdit.Instance.LookToBody(0.5f, "Bip01 Pelvis");
                }
            }

            if (GUI.Button(new Rect(windowWidth - (windowWidth / 4) - padding, windowWidth / 4, windowWidth / 4, windowWidth / 4), "→"))
            {
                if(nowMaid < maidCount - 1)
                {
                    nowMaid++;
                    while (maidList[nowMaid] == null)
                    {
                        nowMaid++;
                        if(nowMaid == maidCount - 1 && maidList[nowMaid] == null)
                        {
                            nowMaid = 0;
                            break;
                        }
                    }
                    SceneEdit.Instance.m_editMaid = maidList[nowMaid];
                    SceneEdit.Instance.LookToBody(0.5f, "Bip01 Pelvis");
                }
            }

            if (GUI.Button(new Rect(padding, windowHeight - ((windowHeight / 8) * 4) - 35F, windowWidth - (padding * 2), windowHeight / 8 - padding), "位置プリセット保存"))
            {
                showSavePresetWindow = !showSavePresetWindow;
            }

            if (GUI.Button(new Rect(padding, windowHeight - ((windowHeight / 8) * 3) - 20F, windowWidth - (padding * 2), windowHeight / 8 - padding), "メイド位置リセット"))
            {
                SceneEdit.Instance.EditMaid.SetPos(new Vector3(0, 0, 0));
            }

            if (GUI.Button(new Rect(padding, windowHeight - ((windowHeight / 8) * 2) - 10F, windowWidth - (padding * 2), windowHeight / 8 - padding), "メイド角度リセット"))
            {
                SceneEdit.Instance.EditMaid.SetRot(new Vector3(0, 0, 0));
            }

            if (GUI.Button(new Rect(padding, windowHeight - (windowHeight / 8), windowWidth - (padding * 2), windowHeight / 8 - padding), "メイド削除")) {
                showSpawnUI = false;
                PositionChanger.setShow(false);
                if(nowMaid != 0)
                {
                    showMaidDeleteWindow = !showMaidDeleteWindow;
                }
            }

            if (GUI.Button(new Rect(padding, windowHeight - ((windowHeight / 8) - (windowHeight / 8) - 20), windowWidth - (padding * 2), windowHeight / 8 - padding), "衣服プリセット保存"))
            {
                showSaveWearPresetWindow = !showSaveWearPresetWindow;
            }
        }


        private void OnMaidDeleteWindow(int id)
        {
            var windowBaseW = screenBaseWidth * 50;
            var windowBaseH = screenBaseHeight * 15;

            if(GUI.Button(new Rect(windowBaseW / 4 - 25, windowBaseH / 4, windowBaseW / 4, windowBaseW / 8), "削除しない"))
            {
                showMaidDeleteWindow = false;
            }

            if (GUI.Button(new Rect((windowBaseW / 4 * 2) + 25, windowBaseH / 4, windowBaseW / 4, windowBaseW / 8), "削除する"))
            {
                showMaidDeleteWindow = false;

                GameMain.Instance.CharacterMgr.DeactivateMaid(nowMaid);
                SceneEdit.Instance.m_editMaid = maidList[0];
                SceneEdit.Instance.LookToBody();
                maidCount--;
                maidList[nowMaid] = null;
                freeMaidSpace.Add(nowMaid);
                nowMaid = 0;
            }

            
        }

        private void SavePresetWindow(int id)
        {
            var windowBaseW = screenBaseWidth * 50;
            var windowBaseH = screenBaseHeight * 15;
            var editStyle = GUI.skin.GetStyle("textField");
            editStyle.fontSize = 34;
            editStyle.alignment = TextAnchor.MiddleCenter;

            savePresetFileName = GUI.TextField(new Rect(windowBaseW / 4, windowBaseH / 4 - windowBaseW / 16 - 5, windowBaseW / 2, windowBaseW / 8), savePresetFileName, editStyle);

            if (GUI.Button(new Rect(windowBaseW / 4, windowBaseH / 4 + windowBaseW / 16 + 5, windowBaseW / 2, windowBaseW / 8), "保存"))
            {
                string presetDirectory = AFileSystemBase.base_path + App.base_path + App.position_preset_path;
                if(!Directory.Exists(presetDirectory))
                {
                    Directory.CreateDirectory(presetDirectory);
                }

                PositionPreset preset = new PositionPreset();
                preset.position = SceneEdit.Instance.EditMaid.GetPos();
                preset.rotation = SceneEdit.Instance.EditMaid.GetRot();
                preset.anm = SceneEdit.Instance.EditMaid.body0.anist.name;
                preset.faceAnime = SceneEdit.Instance.EditMaid.FaceName;
                preset.faceBlend = SceneEdit.Instance.EditMaid.FaceName3;

                StreamWriter streamWriter = new StreamWriter(presetDirectory + "/" + savePresetFileName + ".json");
                streamWriter.Write(JsonUtility.ToJson(preset));
                streamWriter.Close();

                showSavePresetWindow = false;
            }
        }

        private void LoadPositionPresetUI()
        {
            var buttonStyle = GUI.skin.GetStyle("button");
            buttonStyle.fontSize = 34;

            int maxPage = presetFiles.Length / 3;
            if (presetPage > 0)
            {
                if (GUI.Button(new Rect(Screen.width - (screenBaseWidth * 35) - 10, screenBaseHeight * 50, screenBaseWidth * 35 / 4, screenBaseWidth * 35 / 4), "←", buttonStyle))
                {
                    presetPage -= 1;
                }
            }

            if (presetPage < maxPage)
            {
                if (GUI.Button(new Rect(Screen.width - (screenBaseWidth * 35) + (screenBaseWidth * 35 / 4), screenBaseHeight * 50, screenBaseWidth * 35 / 4, screenBaseWidth * 35 / 4), "→", buttonStyle))
                {
                    presetPage += 1;
                }
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 60, screenBaseWidth * 90, screenBaseHeight * 5), Path.GetFileNameWithoutExtension(presetFiles[0 + (3 * presetPage)])))
            {
                StreamReader reader = new StreamReader(presetFiles[0 + (3 * presetPage)]);
                string jsonStr = reader.ReadToEnd();
                PositionPreset preset = JsonUtility.FromJson<PositionPreset>(jsonStr);
                Maid editMaid = SceneEdit.Instance.EditMaid;
                editMaid.SetPos(preset.position);
                editMaid.SetRot(preset.rotation);
                editMaid.CrossFade(preset.anm, false, true, false, 0.35f, 1f);
                editMaid.FaceAnime(preset.faceAnime);
                if (preset.faceBlend.Count() < 1)
                {
                    editMaid.FaceBlend("デフォ");
                } 
                else
                {
                    editMaid.FaceBlend(preset.faceBlend);
                }
                reader.Close();
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 65, screenBaseWidth * 90, screenBaseHeight * 5), Path.GetFileNameWithoutExtension(presetFiles[1 + (3 * presetPage)])))
            {
                StreamReader reader = new StreamReader(presetFiles[1 + (3 * presetPage)]);
                string jsonStr = reader.ReadToEnd();
                PositionPreset preset = JsonUtility.FromJson<PositionPreset>(jsonStr);
                Maid editMaid = SceneEdit.Instance.EditMaid;
                editMaid.SetPos(preset.position);
                editMaid.SetRot(preset.rotation);
                editMaid.CrossFade(preset.anm, false, true, false, 0.35f, 1f);
                editMaid.FaceAnime(preset.faceAnime);
                if (preset.faceBlend.Count() < 1)
                {
                    editMaid.FaceBlend("デフォ");
                }
                else
                {
                    editMaid.FaceBlend(preset.faceBlend);
                }
                reader.Close();
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 70, screenBaseWidth * 90, screenBaseHeight * 5), Path.GetFileNameWithoutExtension(presetFiles[2 + (3 * presetPage)])))
            {
                StreamReader reader = new StreamReader(presetFiles[2 + (3 * presetPage)]);
                string jsonStr = reader.ReadToEnd();
                PositionPreset preset = JsonUtility.FromJson<PositionPreset>(jsonStr);
                Maid editMaid = SceneEdit.Instance.EditMaid;
                editMaid.SetPos(preset.position);
                editMaid.SetRot(preset.rotation);
                editMaid.CrossFade(preset.anm, false, true, false, 0.35f, 1f);
                editMaid.FaceAnime(preset.faceAnime);
                if (preset.faceBlend.Count() < 1)
                {
                    editMaid.FaceBlend("デフォ");
                }
                else
                {
                    editMaid.FaceBlend(preset.faceBlend);
                }
                reader.Close();
            }

        }

        private void SaveWearPresetWindow(int id)
        {
            var windowBaseW = screenBaseWidth * 50;
            var windowBaseH = screenBaseHeight * 15;
            var editStyle = GUI.skin.GetStyle("textField");
            editStyle.fontSize = 34;
            editStyle.alignment = TextAnchor.MiddleCenter;

            saveWearPresetFileName = GUI.TextField(new Rect(windowBaseW / 4, windowBaseH / 4 - windowBaseW / 16 - 5, windowBaseW / 2, windowBaseW / 8), saveWearPresetFileName, editStyle);

            if (GUI.Button(new Rect(windowBaseW / 4, windowBaseH / 4 + windowBaseW / 16 + 5, windowBaseW / 2, windowBaseW / 8), "保存"))
            {
                string presetDirectory = AFileSystemBase.base_path + App.base_path + App.wear_preset_path;
                if (!Directory.Exists(presetDirectory))
                {
                    Directory.CreateDirectory(presetDirectory);
                }

                WearPreset wpreset = new WearPreset();
                wpreset.Pwear = SceneEdit.Instance.EditMaid.GetProp(MPN.wear).strFileName;
                wpreset.Pskirt = SceneEdit.Instance.EditMaid.GetProp(MPN.skirt).strFileName;
                wpreset.Pmizugi = SceneEdit.Instance.EditMaid.GetProp(MPN.mizugi).strFileName;
                wpreset.Pbra = SceneEdit.Instance.EditMaid.GetProp(MPN.bra).strFileName;
                wpreset.Ppanz = SceneEdit.Instance.EditMaid.GetProp(MPN.panz).strFileName;
                wpreset.Pstkg = SceneEdit.Instance.EditMaid.GetProp(MPN.stkg).strFileName;
                wpreset.Pshoes = SceneEdit.Instance.EditMaid.GetProp(MPN.shoes).strFileName;
                wpreset.Pheadset = SceneEdit.Instance.EditMaid.GetProp(MPN.headset).strFileName;
                wpreset.Pglove = SceneEdit.Instance.EditMaid.GetProp(MPN.glove).strFileName;
                wpreset.Pacchana = SceneEdit.Instance.EditMaid.GetProp(MPN.acchana).strFileName;
                wpreset.Pacckami = SceneEdit.Instance.EditMaid.GetProp(MPN.acckami).strFileName;
                wpreset.Pacckamisub = SceneEdit.Instance.EditMaid.GetProp(MPN.acckamisub).strFileName;
                wpreset.Paccmimi = SceneEdit.Instance.EditMaid.GetProp(MPN.accmimi).strFileName;
                wpreset.Pacckubi = SceneEdit.Instance.EditMaid.GetProp(MPN.acckubi).strFileName;
                wpreset.Pacckubiwa = SceneEdit.Instance.EditMaid.GetProp(MPN.acckubiwa).strFileName;
                wpreset.Paccheso = SceneEdit.Instance.EditMaid.GetProp(MPN.accheso).strFileName;
                wpreset.Paccude = SceneEdit.Instance.EditMaid.GetProp(MPN.accude).strFileName;
                wpreset.Paccashi = SceneEdit.Instance.EditMaid.GetProp(MPN.accashi).strFileName;
                wpreset.Paccsenaka = SceneEdit.Instance.EditMaid.GetProp(MPN.accsenaka).strFileName;
                wpreset.Paccshippo = SceneEdit.Instance.EditMaid.GetProp(MPN.accshippo).strFileName;
                wpreset.Pmegane = SceneEdit.Instance.EditMaid.GetProp(MPN.megane).strFileName;
                wpreset.Pacchat = SceneEdit.Instance.EditMaid.GetProp(MPN.acchat).strFileName;
                wpreset.Ponepiece = SceneEdit.Instance.EditMaid.GetProp(MPN.onepiece).strFileName;

                StreamWriter streamWriterW = new StreamWriter(presetDirectory + "/" + saveWearPresetFileName + ".json");
                streamWriterW.Write(JsonUtility.ToJson(wpreset));
                streamWriterW.Close();

                showSaveWearPresetWindow = false;
            }
        }

        private void LoadWearPresetUI()
        {
            var buttonStyle = GUI.skin.GetStyle("button");
            buttonStyle.fontSize = 34;

            int maxPage = presetFilesW.Length / 3;
            if (WpresetPage > 0)
            {
                if (GUI.Button(new Rect(Screen.width - (screenBaseWidth * 35) - 10, screenBaseHeight * 50, screenBaseWidth * 35 / 4, screenBaseWidth * 35 / 4), "←", buttonStyle))
                {
                    WpresetPage -= 1;
                }
            }

            if (WpresetPage < maxPage)
            {
                if (GUI.Button(new Rect(Screen.width - (screenBaseWidth * 35) + (screenBaseWidth * 35 / 4), screenBaseHeight * 50, screenBaseWidth * 35 / 4, screenBaseWidth * 35 / 4), "→", buttonStyle))
                {
                    WpresetPage += 1;
                }
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 60, screenBaseWidth * 90, screenBaseHeight * 5), Path.GetFileNameWithoutExtension(presetFilesW[0 + (3 * WpresetPage)])))
            {
                StreamReader reader = new StreamReader(presetFilesW[0 + (3 * WpresetPage)]);
                string jsonStr = reader.ReadToEnd();
                WearPreset preset = JsonUtility.FromJson<WearPreset>(jsonStr);
                Maid editMaid = SceneEdit.Instance.EditMaid;
                editMaid.SetProp(MPN.wear, preset.Pwear, 0);
                editMaid.SetProp(MPN.skirt, preset.Pskirt, 0);
                editMaid.SetProp(MPN.mizugi, preset.Pmizugi, 0);
                editMaid.SetProp(MPN.bra, preset.Pbra, 0);
                editMaid.SetProp(MPN.panz, preset.Ppanz, 0);
                editMaid.SetProp(MPN.stkg, preset.Pstkg, 0);
                editMaid.SetProp(MPN.shoes, preset.Pshoes, 0);
                editMaid.SetProp(MPN.headset, preset.Pheadset, 0);
                editMaid.SetProp(MPN.glove, preset.Pglove, 0);
                editMaid.SetProp(MPN.acchana, preset.Pacchana, 0);
                editMaid.SetProp(MPN.acckami, preset.Pacckami, 0);
                editMaid.SetProp(MPN.acckamisub, preset.Pacckamisub, 0);
                editMaid.SetProp(MPN.accmimi, preset.Paccmimi, 0);
                editMaid.SetProp(MPN.acckubi, preset.Pacckubi, 0);
                editMaid.SetProp(MPN.acckubiwa, preset.Pacckubiwa, 0);
                editMaid.SetProp(MPN.accheso, preset.Paccheso, 0);
                editMaid.SetProp(MPN.accude, preset.Paccude, 0);
                editMaid.SetProp(MPN.accashi, preset.Paccashi, 0);
                editMaid.SetProp(MPN.accsenaka, preset.Paccsenaka, 0);
                editMaid.SetProp(MPN.accshippo, preset.Paccshippo, 0);
                editMaid.SetProp(MPN.megane, preset.Pmegane, 0);
                editMaid.SetProp(MPN.acchat, preset.Pacchat, 0);
                editMaid.SetProp(MPN.onepiece, preset.Ponepiece, 0);
                editMaid.AllProcProp();
                reader.Close();
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 65, screenBaseWidth * 90, screenBaseHeight * 5), Path.GetFileNameWithoutExtension(presetFilesW[1 + (3 * WpresetPage)])))
            {
                StreamReader reader = new StreamReader(presetFilesW[1 + (3 * WpresetPage)]);
                string jsonStr = reader.ReadToEnd();
                WearPreset preset = JsonUtility.FromJson<WearPreset>(jsonStr);
                Maid editMaid = SceneEdit.Instance.EditMaid;
                editMaid.SetProp(MPN.wear, preset.Pwear, 0);
                editMaid.SetProp(MPN.skirt, preset.Pskirt, 0);
                editMaid.SetProp(MPN.mizugi, preset.Pmizugi, 0);
                editMaid.SetProp(MPN.bra, preset.Pbra, 0);
                editMaid.SetProp(MPN.panz, preset.Ppanz, 0);
                editMaid.SetProp(MPN.stkg, preset.Pstkg, 0);
                editMaid.SetProp(MPN.shoes, preset.Pshoes, 0);
                editMaid.SetProp(MPN.headset, preset.Pheadset, 0);
                editMaid.SetProp(MPN.glove, preset.Pglove, 0);
                editMaid.SetProp(MPN.acchana, preset.Pacchana, 0);
                editMaid.SetProp(MPN.acckami, preset.Pacckami, 0);
                editMaid.SetProp(MPN.acckamisub, preset.Pacckamisub, 0);
                editMaid.SetProp(MPN.accmimi, preset.Paccmimi, 0);
                editMaid.SetProp(MPN.acckubi, preset.Pacckubi, 0);
                editMaid.SetProp(MPN.acckubiwa, preset.Pacckubiwa, 0);
                editMaid.SetProp(MPN.accheso, preset.Paccheso, 0);
                editMaid.SetProp(MPN.accude, preset.Paccude, 0);
                editMaid.SetProp(MPN.accashi, preset.Paccashi, 0);
                editMaid.SetProp(MPN.accsenaka, preset.Paccsenaka, 0);
                editMaid.SetProp(MPN.accshippo, preset.Paccshippo, 0);
                editMaid.SetProp(MPN.megane, preset.Pmegane, 0);
                editMaid.SetProp(MPN.acchat, preset.Pacchat, 0);
                editMaid.SetProp(MPN.onepiece, preset.Ponepiece, 0);
                editMaid.AllProcProp();
                reader.Close();
            }

            if (GUI.Button(new Rect(10, screenBaseHeight * 70, screenBaseWidth * 90, screenBaseHeight * 5), Path.GetFileNameWithoutExtension(presetFilesW[2 + (3 * WpresetPage)])))
            {
                StreamReader reader = new StreamReader(presetFilesW[2 + (3 * WpresetPage)]);
                string jsonStr = reader.ReadToEnd();
                WearPreset preset = JsonUtility.FromJson<WearPreset>(jsonStr);
                Maid editMaid = SceneEdit.Instance.EditMaid;
                editMaid.SetProp(MPN.wear, preset.Pwear, 0);
                editMaid.SetProp(MPN.skirt, preset.Pskirt, 0);
                editMaid.SetProp(MPN.mizugi, preset.Pmizugi, 0);
                editMaid.SetProp(MPN.bra, preset.Pbra, 0);
                editMaid.SetProp(MPN.panz, preset.Ppanz, 0);
                editMaid.SetProp(MPN.stkg, preset.Pstkg, 0);
                editMaid.SetProp(MPN.shoes, preset.Pshoes, 0);
                editMaid.SetProp(MPN.headset, preset.Pheadset, 0);
                editMaid.SetProp(MPN.glove, preset.Pglove, 0);
                editMaid.SetProp(MPN.acchana, preset.Pacchana, 0);
                editMaid.SetProp(MPN.acckami, preset.Pacckami, 0);
                editMaid.SetProp(MPN.acckamisub, preset.Pacckamisub, 0);
                editMaid.SetProp(MPN.accmimi, preset.Paccmimi, 0);
                editMaid.SetProp(MPN.acckubi, preset.Pacckubi, 0);
                editMaid.SetProp(MPN.acckubiwa, preset.Pacckubiwa, 0);
                editMaid.SetProp(MPN.accheso, preset.Paccheso, 0);
                editMaid.SetProp(MPN.accude, preset.Paccude, 0);
                editMaid.SetProp(MPN.accashi, preset.Paccashi, 0);
                editMaid.SetProp(MPN.accsenaka, preset.Paccsenaka, 0);
                editMaid.SetProp(MPN.accshippo, preset.Paccshippo, 0);
                editMaid.SetProp(MPN.megane, preset.Pmegane, 0);
                editMaid.SetProp(MPN.acchat, preset.Pacchat, 0);
                editMaid.SetProp(MPN.onepiece, preset.Ponepiece, 0);
                editMaid.AllProcProp();
                reader.Close();
            }

        }

        private void SpawnUI()
        {
            var buttonStyle = GUI.skin.GetStyle("button");
            buttonStyle.fontSize = 34;

            int maxPage = maidFileList.Count / 3;
            if (spawnPage > 0)
            {
                if (GUI.Button(new Rect(Screen.width - (screenBaseWidth * 35) - 10, screenBaseHeight * 50, screenBaseWidth * 35 / 4, screenBaseWidth * 35 / 4), "←", buttonStyle))
                {
                    spawnPage -= 1;
                }
            }

            if(spawnPage < maxPage)
            {
                if (GUI.Button(new Rect(Screen.width - (screenBaseWidth * 35) + (screenBaseWidth * 35 / 4), screenBaseHeight * 50, screenBaseWidth * 35 / 4, screenBaseWidth * 35 / 4), "→", buttonStyle))
                {
                    spawnPage += 1;
                }
            }

            if(GUI.Button(new Rect(10, screenBaseHeight * 60, screenBaseWidth * 90, screenBaseHeight * 5), maidFileList[0 + (3 * spawnPage)].status.m_lastName + " " + maidFileList[0 + (3 * spawnPage)].status.m_firstName))
            {
                if(freeMaidSpace.Count > 0)
                {
                    maidList[freeMaidSpace[0]] = GameMain.Instance.CharacterMgr.AddStockMaid();
                    GameMain.Instance.CharacterMgr.MaidFileLoad(maidFileList[0 + (3 * spawnPage)], maidList[freeMaidSpace[0]]);
                    GameMain.Instance.CharacterMgr.SetActiveMaid(maidList[freeMaidSpace[0]], freeMaidSpace[0]);
                    maidList[freeMaidSpace[0]].AllProcPropSeqStart();
                    maidList[freeMaidSpace[0]].Visible = true;
                    freeMaidSpace.RemoveAt(0);
                    maidCount++;
                }
                else
                {
                    maidList[maidCount] = GameMain.Instance.CharacterMgr.AddStockMaid();
                    GameMain.Instance.CharacterMgr.MaidFileLoad(maidFileList[0 + (3 * spawnPage)], maidList[maidCount]);
                    GameMain.Instance.CharacterMgr.SetActiveMaid(maidList[maidCount], maidCount);
                    maidList[maidCount].AllProcPropSeqStart();
                    maidList[maidCount].Visible = true;
                    maidCount++;
                }
                
            }
            if(GUI.Button(new Rect(10, screenBaseHeight * 65, screenBaseWidth * 90, screenBaseHeight * 5), maidFileList[1 + (3 * spawnPage)].status.m_lastName + " " + maidFileList[1 + (3 * spawnPage)].status.m_firstName)) {
                if(freeMaidSpace.Count > 0)
                {
                    maidList[freeMaidSpace[0]] = GameMain.Instance.CharacterMgr.AddStockMaid();
                    GameMain.Instance.CharacterMgr.MaidFileLoad(maidFileList[1 + (3 * spawnPage)], maidList[freeMaidSpace[0]]);
                    GameMain.Instance.CharacterMgr.SetActiveMaid(maidList[freeMaidSpace[0]], freeMaidSpace[0]);
                    maidList[freeMaidSpace[0]].AllProcPropSeqStart();
                    maidList[freeMaidSpace[0]].Visible = true;
                    freeMaidSpace.RemoveAt(0);
                    maidCount++;
                }
                else
                {
                    maidList[maidCount] = GameMain.Instance.CharacterMgr.AddStockMaid();
                    GameMain.Instance.CharacterMgr.MaidFileLoad(maidFileList[1 + (3 * spawnPage)], maidList[maidCount]);
                    GameMain.Instance.CharacterMgr.SetActiveMaid(maidList[maidCount], maidCount);
                    maidList[maidCount].AllProcPropSeqStart();
                    maidList[maidCount].Visible = true;
                    maidCount++;
                }
            }
            
            if(GUI.Button(new Rect(10, screenBaseHeight * 70, screenBaseWidth * 90, screenBaseHeight * 5), maidFileList[2 + (3 * spawnPage)].status.m_lastName + " " + maidFileList[2 + (3 * spawnPage)].status.m_firstName))
            {
                if (freeMaidSpace.Count > 0)
                {
                    maidList[freeMaidSpace[0]] = GameMain.Instance.CharacterMgr.AddStockMaid();
                    GameMain.Instance.CharacterMgr.MaidFileLoad(maidFileList[2 + (3 * spawnPage)], maidList[freeMaidSpace[0]]);
                    GameMain.Instance.CharacterMgr.SetActiveMaid(maidList[freeMaidSpace[0]], freeMaidSpace[0]);
                    maidList[freeMaidSpace[0]].AllProcPropSeqStart();
                    maidList[freeMaidSpace[0]].Visible = true;
                    freeMaidSpace.RemoveAt(0);
                    maidCount++;
                }
                else
                {
                    maidList[maidCount] = GameMain.Instance.CharacterMgr.AddStockMaid();
                    GameMain.Instance.CharacterMgr.MaidFileLoad(maidFileList[2 + (3 * spawnPage)], maidList[maidCount]);
                    GameMain.Instance.CharacterMgr.SetActiveMaid(maidList[maidCount], maidCount);
                    maidList[maidCount].AllProcPropSeqStart();
                    maidList[maidCount].Visible = true;
                    maidCount++;
                }
            }
        }

        static public void setShow(bool b)
        {
            App.Config.showMaidEditor = b;
        }

        static void shotScreenShot()
        {
            RenderTexture rend_tex = new RenderTexture(Screen.width, Screen.height, 24);
            rend_tex.isPowerOfTwo = false;
            rend_tex.antiAliasing = 0;
            Camera main_cam = GameMain.Instance.MainCamera.camera;
            RenderTexture orijin_rendtex = main_cam.targetTexture;
            RenderTexture active_tex = RenderTexture.active;
            RenderTexture.active = rend_tex;
            main_cam.targetTexture = rend_tex;
            main_cam.Render();
            Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), 0, 0);
            tex.Apply();
            string image_name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            string image_path = SNSShare.GetImagePath(image_name);
            byte[] bytes = tex.EncodeToPNG();
            NativeGallery.SaveImageToGallery(bytes, "CustomCast", image_name, null);
            main_cam.targetTexture = orijin_rendtex;
            RenderTexture.active = active_tex;
            Destroy(rend_tex);
            Destroy(tex);
        }
    }
}
