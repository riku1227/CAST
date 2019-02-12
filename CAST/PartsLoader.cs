using SCENE_EDIT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace CAST
{
    public class PartsLoader
    {
        public static void InitPartsLoader()
        {
            LoadPartsData(AFileSystemBase.base_path + App.base_path + App.parts_mlist_path);
        }

        private static void LoadPartsData(string path)
        {
            if (Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);

                foreach (string file in files)
                {
                    LoadPartsData(file);
                }

                foreach (string directory in directories)
                {
                    LoadPartsData(directory);
                }
            }
            else if (File.Exists(path))
            {
                string extension = Path.GetExtension(path);
                if(extension == ".mlist" || extension == ".csv")
                {
                    Debug.Log("[CAST]Parts Loaded: " + Path.GetFileName(path));
                    List<EditMenuItemData> list = new List<EditMenuItemData>();
                    Dictionary<EditMenuItemData, string> dictionary = new Dictionary<EditMenuItemData, string>();
                    Dictionary<EditMenuItemData, List<string>> dictionary2 = new Dictionary<EditMenuItemData, List<string>>();
                    using (StreamReader streamReader2 = new StreamReader(path))
                    {
                        string empty2 = string.Empty;
                        try
                        {
                            while (streamReader2.Peek() >= 0)
                            {
                                string[] array4 = streamReader2.ReadLine().Split(new char[]
                                {
                                   ','
                                });
                                if (!(array4[0].Substring(0, 2) == "//"))
                                {
                                    EditMenuItemData editMenuItemData2 = new EditMenuItemData();
                                    editMenuItemData2.MenuRID = int.Parse(array4[0]);
                                    editMenuItemData2.MenuFileName = array4[1];
                                    editMenuItemData2.IconFileName = array4[2];
                                    editMenuItemData2.ItemName = array4[3];
                                    editMenuItemData2.Mpn = (MPN)Enum.Parse(typeof(MPN), array4[4]);
                                    editMenuItemData2.MpnColorSet = (MPN)Enum.Parse(typeof(MPN), array4[5]);
                                    editMenuItemData2.ColorSetFilesName = array4[6];
                                    editMenuItemData2.Priority = float.Parse(array4[7]);
                                    string value2 = array4[8];
                                    if (!string.IsNullOrEmpty(value2))
                                    {
                                        dictionary.Add(editMenuItemData2, value2);
                                    }
                                    editMenuItemData2.ParentMenu = null;
                                    string[] array5 = array4[9].Split(new char[]
                                    {
                                        '|'
                                    });
                                    List<string> list3 = new List<string>();
                                    foreach (string text2 in array5)
                                    {
                                        if (!string.IsNullOrEmpty(text2))
                                        {
                                            list3.Add(text2);
                                        }
                                    }
                                    if (list3.Count > 0)
                                    {
                                        dictionary2.Add(editMenuItemData2, list3);
                                    }
                                    editMenuItemData2.IsDelete = bool.Parse(array4[10]);
                                    editMenuItemData2.MultiColorID = (MaidParts.PARTS_COLOR)Enum.Parse(typeof(MaidParts.PARTS_COLOR), array4[11]);
                                    list.Add(editMenuItemData2);
                                    EditMenuItemData.m_itemFileNameDic.Add(editMenuItemData2.MenuFileName, editMenuItemData2);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            streamReader2.BaseStream.Close();
                            throw;
                        }
                    }

                    EditMenuItemData.SortPriority(ref list, true);
                    foreach (EditMenuItemData editMenuItemData2 in list)
                    {
                        if (dictionary.ContainsKey(editMenuItemData2))
                        {
                            editMenuItemData2.ParentMenu = EditMenuItemData.m_itemFileNameDic[dictionary[editMenuItemData2]];
                        }
                        if (dictionary2.ContainsKey(editMenuItemData2))
                        {
                            foreach (string key in dictionary2[editMenuItemData2])
                            {
                                editMenuItemData2.ChildlenMenuList.Add(EditMenuItemData.m_itemFileNameDic[key]);
                            }
                            EditMenuItemData.SortPriority(ref editMenuItemData2.m_childlenMenuList, true);
                        }
                    }
                    foreach (EditMenuItemData editMenuItemData3 in list)
                    {
                        if (!EditMenuItemData.m_itemMenuDic.ContainsKey(editMenuItemData3.Mpn))
                        {
                            EditMenuItemData.m_itemMenuDic.Add(editMenuItemData3.Mpn, new List<EditMenuItemData>());
                        }
                        EditMenuItemData.m_itemMenuDic[editMenuItemData3.Mpn].Add(editMenuItemData3);
                    }
                }
            }
        }
    }
}
