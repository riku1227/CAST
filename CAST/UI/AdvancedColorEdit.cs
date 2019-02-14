using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CAST.UI
{
    public class AdvancedColorEdit : MonoBehaviour
    {
        public static bool init = false;
        public static MaidParts.PARTS_COLOR nowType;
        private string htmlColorCode = "";

        public static void Init()
        {
            var go = new GameObject();
            go.AddComponent<AdvancedColorEdit>();
            init = true;
        }

        void OnGUI()
        {
            if(App.Config.showAdvancedColorPalette)
            {
                int baseW = Screen.width / 100;
                int baseH = Screen.height / 100;
                GUIStyle textField = GUI.skin.textField;
                textField.fontSize = 40;
                textField.alignment = TextAnchor.MiddleCenter;
                GUIStyle labelStyle = GUI.skin.label;
                labelStyle.fontSize = 30;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                if (GUI.Button(new Rect(Screen.width - 200 - 10, baseH * 35, 200, 45), "非表示にする"))
                {
                    App.Config.showAdvancedColorPalette = false;
                }

                /* if (GUI.Button(new Rect(Screen.width - 400 - 20, baseH * 35, 200, 45), "カラーコード取得"))
                {
                    Color color;
                    MaidParts.PARTS_COLOR mainParts = MaidParts.PARTS_COLOR.HAIR;
                    if (nowType.ToString().IndexOf("HAIR") != -1)
                    {
                        mainParts = MaidParts.PARTS_COLOR.HAIR;
                    }
                    else if (nowType.ToString().IndexOf("SKIN") != -1)
                    {
                        mainParts = MaidParts.PARTS_COLOR.SKIN;
                    }
                    else if (nowType.ToString().IndexOf("EYE") != -1)
                    {
                        mainParts = MaidParts.PARTS_COLOR.EYE_L;
                    }

                    MaidParts.PartsColor partsColor = SceneEdit.Instance.EditMaid.Parts.GetPartsColor(mainParts);
                    color = Color.HSVToRGB(partsColor.m_nMainHue / 255, partsColor.m_nMainChroma / 255, partsColor.m_nMainBrightness / 510);
                    htmlColorCode = ColorUtility.ToHtmlStringRGB(color);
                } */

                htmlColorCode = GUI.TextField(new Rect(Screen.width - 410 - 10, baseH * 38, 410, 70), htmlColorCode, textField);

                if (GUI.Button(new Rect(Screen.width - 200 - 10, baseH * 42, 200, 55), "カラー設定"))
                {
                    MaidParts.PARTS_COLOR mainParts = MaidParts.PARTS_COLOR.HAIR;
                    MaidParts.PARTS_COLOR outlineParts = MaidParts.PARTS_COLOR.HAIR_OUTLINE;
                    if(nowType.ToString().IndexOf("HAIR") != -1)
                    {
                        mainParts = MaidParts.PARTS_COLOR.HAIR;
                        outlineParts = MaidParts.PARTS_COLOR.HAIR_OUTLINE;
                    }
                    else if(nowType.ToString().IndexOf("SKIN") != -1)
                    {
                        mainParts = MaidParts.PARTS_COLOR.SKIN;
                        outlineParts = MaidParts.PARTS_COLOR.SKIN_OUTLINE;
                    }
                    else if (nowType.ToString().IndexOf("EYE") != -1)
                    {
                        mainParts = MaidParts.PARTS_COLOR.EYE_L;
                        outlineParts = MaidParts.PARTS_COLOR.EYE_R;
                    }
                    Color color;
                    ColorUtility.TryParseHtmlString(htmlColorCode, out color);
                    MaidParts maidParts = SceneEdit.Instance.EditMaid.Parts;
                    MaidParts.PartsColor partsColor = maidParts.GetPartsColor(mainParts);
                    float num;
                    float num2;
                    float num3;
                    Color.RGBToHSV(color, out num, out num2, out num3);
                    partsColor.m_nMainHue = Mathf.FloorToInt(num * 255f);
                    partsColor.m_nMainChroma = Mathf.FloorToInt(num2 * 255f);
                    partsColor.m_nMainBrightness = Mathf.FloorToInt(num3 * 510f);
                    maidParts.SetPartsColor(mainParts, partsColor);
                    maidParts.SetPartsColor(outlineParts, partsColor);

                }
                string nowTypeName = "";
                if (nowType.ToString().IndexOf("HAIR") != -1)
                {
                    nowTypeName = "HAIR";
                }
                else if (nowType.ToString().IndexOf("SKIN") != -1)
                {
                    nowTypeName = "SKIN";
                }
                else if (nowType.ToString().IndexOf("EYE") != -1)
                {
                    nowTypeName = "EYE";
                }
                GUI.Box(new Rect(Screen.width - 400 - 20, baseH * 42, 200, 55), "");
                GUI.Label(new Rect(Screen.width - 400 - 20, baseH * 42, 200, 55), nowTypeName, labelStyle);
            }
        }
    }
}
