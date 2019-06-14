using SCENE_EDIT;
using System.IO;
using System.Linq;

using UnityEngine;

namespace CAST.Loader
{
    class PoseDataLoader
    {
        public static int poseId = 10000;
        public static void LoadPoseData()
        {
            if(MODManager.fileSystem == null)
            {
                return;
            }

            var poseCSVList = MODManager.fileSystem.loadFilePathList.Where(x => x.Key.IndexOf("pose_list.csv") != -1).ToList();
            
            foreach(var item in poseCSVList)
            {
                using (var reader = new StreamReader(item.Value))
                {
                    while(!reader.EndOfStream)
                    {
                        var csv = reader.ReadLine().Split(',');
                        var csvLength = csv.Length;
                        if (csvLength >= 2)
                        {
                            var editPoseData = new EditPoseData();
                            var editPoseDataType = typeof(EditPoseData);
                            Util.invokePrivateSetter(editPoseDataType, editPoseData, "ID", poseId);
                            Util.invokePrivateSetter(editPoseDataType, editPoseData, "IconFileName", csv[0]);
                            Util.invokePrivateSetter(editPoseDataType, editPoseData, "LockBoneName", "Bip01 Pelvis");
                            Util.invokePrivateSetter(editPoseDataType, editPoseData, "FileName", csv[1]);

                            var face = "通常";
                            var faceBlend = "無し";
                            if(csvLength >= 3)
                            {
                                face = csv[2];
                            }

                            if(csvLength >= 4)
                            {
                                faceBlend = csv[3];
                            }

                            Util.invokePrivateSetter(editPoseDataType, editPoseData, "Face", face);
                            Util.invokePrivateSetter(editPoseDataType, editPoseData, "FaceBlend", faceBlend);

                            EditPoseData.DataList.Add(editPoseData);
                            EditPoseData.EnabledList.Add(poseId);

                            poseId++;
                        }
                    }
                    foreach(var value in EditPoseData.DataList)
                    {
                        Debug.Log("Enabled: " + value.ID + " : " + value.FileName);
                    }
                }
            }
        }
    }
}
