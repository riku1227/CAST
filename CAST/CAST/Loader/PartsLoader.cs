using CAST.data;
using SCENE_EDIT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CAST.Loader
{
    public class PartsLoader
    {
        public static String childRegex = "_z\\d+\\.menu$";
        public static String pororiRegex = "_porori(\\d+|)";
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
                    if(fileNmae.IndexOf("zurashi") == -1 && fileNmae.IndexOf("mekure") == -1 && fileNmae.IndexOf("porori") == -1)
                    {
                        return !Regex.IsMatch(fileNmae, childRegex);
                    }
                }
                return false;
            }
            );

            List<EditMenuItemData> editMenuDataList = new List<EditMenuItemData>();
            Dictionary<EditMenuItemData, string> childMenuDataDictionary = new Dictionary<EditMenuItemData, string>();
            Dictionary<EditMenuItemData, List<string>> parentInChildMenuFileNames = new Dictionary<EditMenuItemData, List<string>>();
            Dictionary<String, List<string>> parentInDiffFileNames = new Dictionary<String, List<string>>();
            Dictionary<String, EditMenuItemData> parentInDiffData = new Dictionary<string, EditMenuItemData>();

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
                            AddMenuItem(menuData, displayMenuFilesName, "", ref editMenuDataList, childMenuDataDictionary, parentInChildMenuFileNames, parentInDiffFileNames);
                            LoadChildMenu(item.Key.Replace("_i_.menu", ".menu").Replace(".menu", ""), item.Key, ref editMenuDataList, childMenuDataDictionary, parentInChildMenuFileNames, parentInDiffFileNames);
                            if(!String.IsNullOrEmpty(menuData.zurashiMenuFileName) || !String.IsNullOrEmpty(menuData.mekureMenuFileName) || !String.IsNullOrEmpty(menuData.mekureBackMenuFileName) || menuData.pororiMenuFilesName.Count > 0)
                            {
                                LoadDiffMenu(parentInDiffFileNames, parentInDiffData, ref editMenuDataList, childMenuDataDictionary, parentInChildMenuFileNames);
                            }
                        }
                    }
                }
            }

            EditMenuItemData.SortPriority(ref editMenuDataList, true);
            foreach (EditMenuItemData editMenuItemData2 in editMenuDataList)
            {
                if (childMenuDataDictionary.ContainsKey(editMenuItemData2))
                {
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

            foreach (var item in parentInDiffFileNames)
            {
                foreach (var value in item.Value)
                {
                    EditMenuItemData.ItemFileNameDic[item.Key].ChildlenMenuList.Add(parentInDiffData[value]);
                }
            }
        }

        public static void GetChildMenu(String baseMenuName, ref String result)
        {
            var childList = MODManager.fileSystem.loadFilePathList.Where(x => 
            {
                var fileName = x.Key.Replace("_i_.menu", ".menu");
                if(Regex.IsMatch(fileName, baseMenuName + childRegex))
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
            ref List<EditMenuItemData> editMenuDataList, Dictionary<EditMenuItemData, string> childMenuDataDictionary, Dictionary<EditMenuItemData, List<string>> parentInChildMenuFileNames, 
            Dictionary<String, List<String>> parentInDiffFileNames)
        {
            var childList = MODManager.fileSystem.loadFilePathList.Where(x =>
            {
                var fileName = x.Key.Replace("_i_.menu", ".menu");
                if (Regex.IsMatch(fileName, parentMenuBaseFileName + childRegex))
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
                            AddMenuItem(menuData, "", parentMenuFileName, ref editMenuDataList, childMenuDataDictionary, parentInChildMenuFileNames, parentInDiffFileNames);
                        }
                    }
                }

            }
        }

        public static void LoadDiffMenu(Dictionary<String, List<string>> parentInDiffFileNames,Dictionary<String, EditMenuItemData> parentInDiffData,
            ref List<EditMenuItemData> editMenuDataList, Dictionary<EditMenuItemData, string> childMenuDataDictionary, Dictionary<EditMenuItemData, List<string>> parentInChildMenuFileNames)
        {
            foreach (var diffFileNames in parentInDiffFileNames)
            {
                foreach(var fileName in diffFileNames.Value)
                {
                    if (MODManager.fileSystem.loadFilePathList.ContainsKey(fileName))
                    {
                        using (var stream = new FileStream(MODManager.fileSystem.loadFilePathList[fileName], FileMode.Open))
                        {
                            using (var binaryReader = new BinaryReader(stream))
                            {
                                var menuData = ReadMenuFile(binaryReader, fileName);
                                if (menuData != null)
                                {
                                    AddMenuItem(menuData, "", fileName, ref editMenuDataList, childMenuDataDictionary, parentInChildMenuFileNames, parentInDiffFileNames, true, parentInDiffData);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static EditMenuItemDataPlus ReadMenuFile(BinaryReader binaryReader, String menuFileName)
        {
            var menuSignature = binaryReader.ReadString();
            if(menuSignature == "CM3D2_MENU")
            {
                var editMenuItemDataPlus = new EditMenuItemDataPlus();
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
                            case "リソース参照":
                                switch(stringList[1])
                                {
                                    case "パンツずらし":
                                        editMenuItemDataPlus.zurashiMenuFileName = stringList[2];
                                        break;
                                    case "めくれスカート":
                                        editMenuItemDataPlus.mekureMenuFileName = stringList[2];
                                        break;
                                    case "めくれスカート後ろ":
                                        editMenuItemDataPlus.mekureBackMenuFileName = stringList[2];
                                        break;
                                }
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
                editMenuItemDataPlus.editMenuItemData = editMenuItemData;

                var baseMenuFileName = menuFileName.Replace("_i_.menu", ".menu").Replace(".menu", "");
                if (baseMenuFileName.IndexOf("zurashi") == -1 && baseMenuFileName.IndexOf("mekure") == -1 && baseMenuFileName.IndexOf("porori") == -1)
                {
                    var pororiMenuDic = MODManager.fileSystem.loadFilePathList.Where(x => {
                        var fileNmae = x.Key.Replace("_i_.menu", ".menu").Replace("_i_", "");
                        if(Regex.IsMatch(fileNmae, baseMenuFileName + pororiRegex))
                        {
                            if(Path.GetExtension(fileNmae) == ".menu")
                            {
                                return true;
                            }
                        }
                        return false;
                    });

                    var pororiListKeyValue = pororiMenuDic.ToList();
                    var pororiList = new List<String>();
                    foreach(var item in pororiListKeyValue)
                    {
                        pororiList.Add(item.Key);
                    }
                    editMenuItemDataPlus.pororiMenuFilesName = pororiList;
                }

                return editMenuItemDataPlus;
            }
            else
            {
                return null;
            }
        }

        public static void AddMenuItem(
            EditMenuItemDataPlus editMenuItemDataPlus, String displayMenuFiles, String parentMenuFileName,
            ref List<EditMenuItemData> editMenuDataList, Dictionary<EditMenuItemData, string> childMenuDataDictionary, Dictionary<EditMenuItemData, List<string>> parentInChildMenuFileNames, 
            Dictionary<String, List<String>> parentInDiffFileNames, bool isDiffParts = false, Dictionary<String, EditMenuItemData> parentInDiffData = null)
        {
            if (!string.IsNullOrEmpty(parentMenuFileName))
            {
                childMenuDataDictionary.Add(editMenuItemDataPlus.editMenuItemData, editMenuItemDataPlus.editMenuItemData.MenuFileName);
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
                parentInChildMenuFileNames.Add(editMenuItemDataPlus.editMenuItemData, list2);
            }

            if (String.IsNullOrEmpty(parentMenuFileName))
            {
                parentMenuFileName = editMenuItemDataPlus.editMenuItemData.MenuFileName;
            }

            if (!String.IsNullOrEmpty(editMenuItemDataPlus.zurashiMenuFileName))
            {
                AddDiffFile(parentMenuFileName, editMenuItemDataPlus.zurashiMenuFileName, parentInDiffFileNames);
            }

            if (!String.IsNullOrEmpty(editMenuItemDataPlus.mekureMenuFileName))
            {
                AddDiffFile(parentMenuFileName, editMenuItemDataPlus.mekureMenuFileName, parentInDiffFileNames);
            }

            if (!String.IsNullOrEmpty(editMenuItemDataPlus.mekureBackMenuFileName))
            {
                AddDiffFile(parentMenuFileName, editMenuItemDataPlus.mekureBackMenuFileName, parentInDiffFileNames);
            }

            if(editMenuItemDataPlus.pororiMenuFilesName != null)
            {
                if (editMenuItemDataPlus.pororiMenuFilesName.Count() > 0)
                {
                    foreach (var item in editMenuItemDataPlus.pororiMenuFilesName)
                    {
                        AddDiffFile(parentMenuFileName, item, parentInDiffFileNames);
                    }
                }
            }

            if (isDiffParts && parentInDiffData != null)
            {
                if(!parentInDiffData.ContainsKey(editMenuItemDataPlus.editMenuItemData.MenuFileName))
                {
                    parentInDiffData.Add(editMenuItemDataPlus.editMenuItemData.MenuFileName, editMenuItemDataPlus.editMenuItemData);
                }
            }

            editMenuDataList.Add(editMenuItemDataPlus.editMenuItemData);
            if(!EditMenuItemData.ItemFileNameDic.ContainsKey(editMenuItemDataPlus.editMenuItemData.MenuFileName))
            {
                EditMenuItemData.ItemFileNameDic.Add(editMenuItemDataPlus.editMenuItemData.MenuFileName, editMenuItemDataPlus.editMenuItemData);
            }
        }

        public static void AddDiffFile(String parentMenuFileName, String diffMenuFileName, Dictionary<String, List<String>> parentInDiffFileNames)
        {
            if (!parentInDiffFileNames.ContainsKey(parentMenuFileName))
            {
                parentInDiffFileNames.Add(parentMenuFileName, new List<string>() { diffMenuFileName });
            }
            else
            {
                parentInDiffFileNames[parentMenuFileName].Add(diffMenuFileName);
            }
        }
    }
}
