using System;
using System.Collections.Generic;
using System.IO;

namespace CAST.FileSystem
{
    class FileSystemStorage : FileSystemAB
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

        public override AFileBase FileOpen(string file_name)
        {   
            file_name = file_name.ToLower();
            FileSystemAB.AssetData assetData;
            var filePath = "";
            var alistDic = Util.getPrivateField(typeof(FileSystemAB), this, "m_fileDatas") as Dictionary<String, FileSystemAB.AssetData>;
            if (alistDic.TryGetValue(file_name, out assetData))
            {
                return new FileAB(file_name, assetData.fileSize, assetData.assetBundle);
            }
            else if (loadFilePathList.TryGetValue(file_name, out filePath))
            {
                return new FileStorage(filePath);
            }
            return null;
        }

        public override bool IsExistentFile(string file_name)
        {
            file_name = file_name.ToLower();
            bool result = false;
            FileSystemAB.AssetData assetData;
            var filePath = "";
            var alistDic = Util.getPrivateField(typeof(FileSystemAB), this, "m_fileDatas") as Dictionary<String, FileSystemAB.AssetData>;
            if (alistDic.TryGetValue(file_name, out assetData))
            {
                result = assetData.assetBundle.Contains(file_name);
            }
            else if (loadFilePathList.TryGetValue(file_name, out filePath))
            {
                result = File.Exists(filePath);
            }
            return result;
        }

        public Dictionary<String, String> loadFilePathList = new Dictionary<String, String>();
    }
}
