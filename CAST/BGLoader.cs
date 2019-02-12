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
    public class BGLoader : MonoBehaviour
    {
        public static void InitBGLoader()
        {
            LoadBGData(AFileSystemBase.base_path + App.base_path + App.bg_csv_path);
        }

        private static void LoadBGData(string path)
        {
            if (Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);

                foreach (string file in files)
                {
                    LoadBGData(file);
                }

                foreach (string directory in directories)
                {
                    LoadBGData(directory);
                }
            }
            else if (File.Exists(path))
            {
                string extension = Path.GetExtension(path);
                if (extension == ".mlist" || extension == ".csv")
                {
                    if (path.IndexOf("enabled") == -1)
                    {
                        using (StreamReader sr = new StreamReader(path))
                        {
                            while (!sr.EndOfStream)
                            {
                                string[] values = sr.ReadLine().Split(new char[]
                                {
                                    ','
                                });
                                if (!(values[0].Substring(0, 2) == "//"))
                                {
                                    EditBgData editBgData = new EditBgData();
                                    editBgData.ID = int.Parse(values[0]);
                                    editBgData.FileName = values[1];
                                    editBgData.IconFileName = values[2];
                                    editBgData.Name = values[3];
                                    editBgData.Type = values[4];
                                    EditBgData.m_dataList.Add(editBgData);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (StreamReader sr2 = new StreamReader(path))
                        {
                            while (!sr2.EndOfStream)
                            {
                                string line = sr2.ReadLine();
                                if (line != "")
                                {
                                    if (!(line.Substring(0, 2) == "//"))
                                    {
                                        EditBgData.m_enabledList.Add(int.Parse(line));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
