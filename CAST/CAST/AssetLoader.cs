using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CAST
{
    public class AssetLoader
    {
        public FileSystemAB fileSystem;
        public AssetLoader(FileSystemAB fileSystem)
        {
            this.fileSystem = fileSystem;
        }


        public void loadDirectory(String path)
        {
            if (Directory.Exists(path))
            {
                var dirList = Directory.GetDirectories(path);
                foreach(var dir in dirList)
                {
                    loadDirectory(dir);
                }
                var fileList = Directory.GetFiles(path);
                foreach(var file in fileList)
                {
                    loadDirectory(file);
                }
            }
            else if(File.Exists(path))
            {
                var split = path.Split('.');
                if (split[split.Length - 1] == "assets")
                {
                    AddFolder(path.Replace(".assets", ""));
                }
            }
        }
        public void AddFolder(String path)
        {
            String assetsPath = path.ToLower() + ".assets";
            String alistPath = assetsPath + ".alist";
            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetsPath);
            if(assetsPath == null)
            {
                return;
            }
            var assetBundleDic = Util.getPrivateField(typeof(FileSystemAB), fileSystem, "m_assetBundle") as Dictionary<String, AssetBundle>;
            assetBundleDic[Path.GetFileNameWithoutExtension(assetsPath)] = assetBundle;
            Util.setPrivateField(typeof(FileSystemAB), fileSystem, "m_assetBundle", assetBundleDic);
            using (StreamReader streamReader = new StreamReader(alistPath, Encoding.GetEncoding("utf-8")))
            {
                string tempText;
                while ((tempText = streamReader.ReadLine()) != null)
                {
                    string[] array3 = tempText.Split(new char[]
                    {
                    ','
                    });
                    string text4 = array3[1];
                    string filePath = array3[2];
                    ulong fileSize = ulong.Parse(array3[3]);
                    var alistDic = Util.getPrivateField(typeof(FileSystemAB), fileSystem, "m_fileDatas") as Dictionary<String, FileSystemAB.AssetData>;
                    alistDic[text4] = new FileSystemAB.AssetData
                    {
                        assetBundle = assetBundle,
                        fileName = text4,
                        filePath = filePath,
                        fileSize = fileSize
                    };
                    Util.setPrivateField(typeof(FileSystemAB), fileSystem, "m_fileDatas", alistDic);
                }
            }
        }
    }
}
