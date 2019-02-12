using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SCENE_EDIT;

using UnityEngine;

namespace CAST
{
    public class PoseLoader : MonoBehaviour
    {
        public static void LoadAssets(FileSystemAB fileSystemAB)
        {
            Debug.Log("[CAST]" + fileSystemAB.ToString());
            App.fileSystemAB = fileSystemAB;
        }
        public static void InitPoseLoader()
        {
            LoadPoseData(AFileSystemBase.base_path + App.base_path + App.pose_csv_path);
            Debug.Log("[CAST] PoseCount:" + EditPoseData.m_dataList.Count());
        }

        private static void LoadPoseData(string path)
        {
            if(Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);

                foreach(string file in files)
                {
                    LoadPoseData(file);
                }

                foreach(string directory in directories)
                {
                    LoadPoseData(directory);
                }
            } else if (File.Exists(path))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                if(path.IndexOf("enabled") == -1)
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (!sr.EndOfStream)
                        {
                            string[] values = sr.ReadLine().Split(new char[]
                            {
                                ','
                            });
                            if(values.Length == 6)
                            {
                                if (!(values[0].Substring(0, 2) == "//"))
                                {
                                    EditPoseData editPoseData2 = new EditPoseData();
                                    editPoseData2.ID = int.Parse(values[0]);
                                    editPoseData2.IconFileName = values[1];
                                    editPoseData2.LockBoneName = values[2];
                                    editPoseData2.FileName = values[3];
                                    editPoseData2.Face = values[4];
                                    editPoseData2.FaceBlend = values[5];
                                    EditPoseData.m_dataList.Add(editPoseData2);
                                }
                            }
                        }
                    }
                } else
                {
                    using (StreamReader sr2 = new StreamReader(path))
                    {
                        while (!sr2.EndOfStream)
                        {
                            string line = sr2.ReadLine();
                            if(line != "")
                            {
                                if (!(line.Substring(0, 2) == "//"))
                                {
                                    EditPoseData.m_enabledList.Add(int.Parse(line));
                                }
                            }
                        }
                    }
                }
            }
        }
    } 
}
