using SCENE_EDIT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CAST.Loader
{
    public class PartsLoader
    {
        public static void LoadParts()
        {
            if (MODManager.fileSystem == null)
            {
                return;
            }

            var baseMenuDic = MODManager.fileSystem.loadFilePathList.Where(x =>
            {
                var fileNmae = x.Key.Replace("_i_.menu", ".menu");
                if(Path.GetExtension(fileNmae) == ".menu")
                {
                    if(fileNmae.IndexOf("_zurashi") == -1 && fileNmae.IndexOf("_mekure") == -1 || fileNmae.IndexOf("_porori") == -1)
                    {
                        return !Regex.IsMatch(x.Key, "_z\\d+\\.menu$");
                    }
                }
                return false;
            }
            );

            List<EditMenuItemData> editMenuDataList = new List<EditMenuItemData>();
            Dictionary<EditMenuItemData, string> childMenuDataDictionary = new Dictionary<EditMenuItemData, string>();
            Dictionary<EditMenuItemData, List<string>> parentInChildMenuFileNames = new Dictionary<EditMenuItemData, List<string>>();

            foreach (var item in baseMenuDic)
            {
                using (var stream = new FileStream(item.Value, FileMode.Open))
                {
                    using (var binaryReader = new BinaryReader(stream))
                    {
                        var displayMenuFilesName = item.Key;


                        GetChildMenu(item.Key.Replace("_i_.menu", ".menu").Replace(".menu", ""), ref displayMenuFilesName);
                        var menuData = ReadMenuFile(binaryReader, item.Key);
                        if(menuData != null)
                        {
                            AddMenuItem(menuData, displayMenuFilesName, "", ref editMenuDataList, ref childMenuDataDictionary, ref parentInChildMenuFileNames);
                            LoadChildMenu(item.Key.Replace("_i_.menu", ".menu").Replace(".menu", ""), item.Key, ref editMenuDataList, ref childMenuDataDictionary, ref parentInChildMenuFileNames);
                        }
                    }
                }
            }

            EditMenuItemData.SortPriority(ref editMenuDataList, true);
            foreach (EditMenuItemData editMenuItemData2 in editMenuDataList)
            {
                if (childMenuDataDictionary.ContainsKey(editMenuItemData2))
                {
                    Debug.Log(editMenuItemData2.ItemName);
                    Util.invokePrivateSetter(typeof(EditMenuItemData), editMenuItemData2, "ParentMenu", EditMenuItemData.ItemFileNameDic[childMenuDataDictionary[editMenuItemData2]]);
                }
                if (parentInChildMenuFileNames.ContainsKey(editMenuItemData2))
                {
                    foreach (string key in parentInChildMenuFileNames[editMenuItemData2])
                    {
                        editMenuItemData2.ChildlenMenuList.Add(EditMenuItemData.ItemFileNameDic[key]);
                    }
                    EditMenuItemData.SortPriority(ref editMenuItemData2.m_childlenMenuList, true);
                }
            }
            foreach (EditMenuItemData editMenuItemData3 in editMenuDataList)
            {
                if (!EditMenuItemData.ItemMenuDic.ContainsKey(editMenuItemData3.Mpn))
                {
                    EditMenuItemData.ItemMenuDic.Add(editMenuItemData3.Mpn, new List<EditMenuItemData>());
                }
                EditMenuItemData.ItemMenuDic[editMenuItemData3.Mpn].Add(editMenuItemData3);
            }
        }

        public static void GetChildMenu(String baseMenuName, ref String result)
        {
            var childList = MODManager.fileSystem.loadFilePathList.Where(x => 
            {
                var fileName = x.Key.Replace("_i_.menu", ".menu");
                if(Regex.IsMatch(fileName, baseMenuName + "_z\\d+\\.menu$"))
                {
                    return true;
                }
                return false;
            });

            if(childList.Count() > 0)
            {
                result += "|";
                foreach (var item in childList)
                {
                    result += item.Key + "|";
                }
            }
        }

        public static void LoadChildMenu(String parentMenuBaseFileName, String parentMenuFileName,
            ref List<EditMenuItemData> editMenuDataList, ref Dictionary<EditMenuItemData, string> childMenuDataDictionary, ref Dictionary<EditMenuItemData, List<string>> parentInChildMenuFileNames)
        {
            var childList = MODManager.fileSystem.loadFilePathList.Where(x =>
            {
                var fileName = x.Key.Replace("_i_.menu", ".menu");
                if (Regex.IsMatch(fileName, parentMenuBaseFileName + "_z\\d+\\.menu$"))
                {
                    return true;
                }
                return false;
            });

            foreach (var item in childList)
            {
                using (var stream = new FileStream(item.Value, FileMode.Open))
                {
                    using (var binaryReader = new BinaryReader(stream))
                    {
                        var menuData = ReadMenuFile(binaryReader, item.Key);
                        if(menuData != null)
                        {
                            AddMenuItem(menuData, "", parentMenuFileName, ref editMenuDataList, ref childMenuDataDictionary, ref parentInChildMenuFileNames);
                        }
                    }
                }

            }
        }

        public static EditMenuItemData ReadMenuFile(BinaryReader binaryReader, String menuFileName)
        {
            var menuSignature = binaryReader.ReadString();
            if(menuSignature == "CM3D2_MENU")
            {
                var menuVersion = binaryReader.ReadInt32();
                var txtPath = binaryReader.ReadString();
                var partsName = binaryReader.ReadString();
                var partsCategory = binaryReader.ReadString();
                var partsDescription = binaryReader.ReadString();
                binaryReader.ReadInt32();
                var iconFileName = "";
                var priority = 0F;
                var mpnColorSet = "null_mpn";
                var colorSetFilesName = "";
                var menuId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                var isDelete = false;
                var multiColorID = "NONE";

                var isNotEnd = true;
                while (isNotEnd)
                {
                    var num = binaryReader.ReadByte();
                    var text = "";
                    for (int i = 0; i < num; i++)
                    {
                        text = text + "\"" + binaryReader.ReadString() + "\" ";
                    }

                    if (text != string.Empty)
                    {
                        var stringCom = UTY.GetStringCom(text);
                        var stringList = UTY.GetStringList(text);
                        switch (stringCom)
                        {
                            case "icons":
                                iconFileName = stringList[1];
                                break;
                            case "priority":
                                priority = float.Parse(stringList[1]);
                                break;
                            case "color_set":
                                mpnColorSet = stringList[1];
                                colorSetFilesName = stringList[2];
                                break;
                        }
                    }
                    else
                    {
                        isNotEnd = false;
                    }
                }

                var editMenuItemData = new EditMenuItemData();
                var editMenuItemDataType = typeof(EditMenuItemData);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "MenuRID", menuId);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "MenuFileName", menuFileName);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "IconFileName", iconFileName);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "ItemName", partsName);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "Mpn", (MPN)Enum.Parse(typeof(MPN), partsCategory));
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "MpnColorSet", (MPN)Enum.Parse(typeof(MPN), mpnColorSet));
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "ColorSetFilesName", colorSetFilesName);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "Priority", priority);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "ParentMenu", null);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "IsDelete", isDelete);
                Util.invokePrivateSetter(editMenuItemDataType, editMenuItemData, "MultiColorID", (MaidParts.PARTS_COLOR)Enum.Parse(typeof(MaidParts.PARTS_COLOR), multiColorID));
                return editMenuItemData;
            }
            else
            {
                return null;
            }
        }

        public static void AddMenuItem(
            EditMenuItemData editMenuItemData, String displayMenuFiles, String parentMenuFileName,
            ref List<EditMenuItemData> editMenuDataList, ref Dictionary<EditMenuItemData, string> childMenuDataDictionary, ref Dictionary<EditMenuItemData, List<string>> parentInChildMenuFileNames)
        {
            if (!string.IsNullOrEmpty(parentMenuFileName))
            {
                childMenuDataDictionary.Add(editMenuItemData, editMenuItemData.MenuFileName);
            }
            string[] array2 = displayMenuFiles.Split(new char[]
            {
                            '|'
            });
            List<string> list2 = new List<string>();
            foreach (string text2 in array2)
            {
                if (!string.IsNullOrEmpty(text2))
                {
                    list2.Add(text2);
                }
            }
            if (list2.Count > 0)
            {
                parentInChildMenuFileNames.Add(editMenuItemData, list2);
            }

            editMenuDataList.Add(editMenuItemData);
            EditMenuItemData.ItemFileNameDic.Add(editMenuItemData.MenuFileName, editMenuItemData);
        }
    }
}
