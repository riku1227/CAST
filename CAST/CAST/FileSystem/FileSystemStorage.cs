using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CAST.FileSystem
{
    public class FileSystemStorage : FileSystemAB
    {
        public FileSystemStorage() : base()
        {
            loadDirectoryAndFile(AFileSystemBase.base_path + "CAST/");
        }

        public void loadDirectoryAndFile(String path)
        {
            if(Directory.Exists(path))
            {
                var dirList = Directory.GetDirectories(path);
                foreach (var dir in dirList)
                {
                    loadDirectoryAndFile(dir);
                }
                var fileList = Directory.GetFiles(path);
                foreach (var file in fileList)
                {
                    loadDirectoryAndFile(file);
                }
            }
            else if(File.Exists(path))
            {
                if(Path.GetExtension(path) != ".assets" && Path.GetExtension(path) != ".alist")
                {
                    loadFilePathList[Path.GetFileName(path)] = path;
                }
            }
        }

        public override Texture GetTexture(string file_name)
        {
            file_name = Path.GetFileNameWithoutExtension(file_name) + ".png";
            var lowerFileName = file_name.ToLower();
            FileSystemAB.AssetData assetData;
            var filePath = "";
            var alistDic = Util.getPrivateField(typeof(FileSystemAB), this, "m_fileDatas") as Dictionary<String, FileSystemAB.AssetData>;
            if (alistDic.TryGetValue(lowerFileName, out assetData))
            {
                return assetData.assetBundle.LoadAsset<Texture2D>(lowerFileName);
            }
            else if (loadFilePathList.TryGetValue(file_name, out filePath))
            {
                return PNGToTexture2D(filePath);
            }
            return null;
        }

        public override AFileBase FileOpen(string file_name)
        {   
            var lowerFileName = file_name.ToLower();
            FileSystemAB.AssetData assetData;
            var filePath = "";
            var alistDic = Util.getPrivateField(typeof(FileSystemAB), this, "m_fileDatas") as Dictionary<String, FileSystemAB.AssetData>;
            if (alistDic.TryGetValue(lowerFileName, out assetData))
            {
                return new FileAB(lowerFileName, assetData.fileSize, assetData.assetBundle);
            }
            else if (loadFilePathList.TryGetValue(file_name, out filePath))
            {
                return new FileStorage(filePath);
            }
            else if(loadFilePathList.TryGetValue(lowerFileName, out filePath))
            {
                return new FileStorage(filePath);
            }
            return null;
        }

        public override bool IsExistentFile(string file_name)
        {
            var lowerFileName = file_name.ToLower();
            bool result = false;
            FileSystemAB.AssetData assetData;
            var filePath = "";
            var alistDic = Util.getPrivateField(typeof(FileSystemAB), this, "m_fileDatas") as Dictionary<String, FileSystemAB.AssetData>;
            if (alistDic.TryGetValue(lowerFileName, out assetData))
            {
                result = assetData.assetBundle.Contains(lowerFileName);
            }
            else if (loadFilePathList.TryGetValue(file_name, out filePath))
            {
                result = File.Exists(filePath);
            }
            return result;
        }

        public Texture2D PNGToTexture2D(String path)
        {
            byte[] value;
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    value = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                }
            }

            var pos = 16;
            int width = 0;
            for (int i = 0; i < 4; i++)
            {
                width = width * 256 + value[pos++];
            }

            int height = 0;
            for (int i = 0; i < 4; i++)
            {
                height = height * 256 + value[pos++];
            }

            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(value);
            return texture;
        }

        public Dictionary<String, String> loadFilePathList = new Dictionary<String, String>();
    }
}
