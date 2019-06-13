using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CAST
{
    class FileStorage : AFileBase
    {
        private String filePath;
        private ulong fileSize;
        private long pos;
        public FileStorage(String filePath)
        {
            this.filePath = filePath;
            if(File.Exists(filePath))
            {
                this.fileSize = (ulong)new FileInfo(filePath).Length;
            }
        }

        public override bool IsValid()
        {
            return File.Exists(filePath);
        }

        public override int Read(ref byte[] f_byBuf, int f_unReadSize)
        {
            if (!this.IsValid())
            {
                f_byBuf = null;
                return 0;
            }

            if (f_byBuf == null || f_byBuf.Length < f_unReadSize)
            {
                f_byBuf = new byte[f_unReadSize];
            }

            var fileInfo = new FileInfo(filePath);
            var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] bs = new byte[fileInfo.Length];
            reader.Read(bs, 0, (int)fileInfo.Length);
            reader.Close();

            Array.Copy(bs, this.pos, f_byBuf, 0L, (long)f_unReadSize);
            long num;
            this.pos += (num = (long)f_unReadSize);
            return (int)num;
        }

        public override byte[] ReadAll()
        {
            if (!this.IsValid())
            {
                return null;
            }
            int f_unPos = this.Tell();
            byte[] array = new byte[this.GetSize()];
            this.Seek(0, true);
            this.Read(ref array, array.Length);
            this.Seek(f_unPos, true);
            return array;
        }

        public override int Seek(int f_unPos, bool absolute_move)
        {
            if (!this.IsValid())
            {
                return 0;
            }
            if (absolute_move)
            {
                this.pos = (long)f_unPos;
            }
            else
            {
                this.pos += (long)f_unPos;
            }
            return this.Tell();
        }

        public override int Tell()
        {
            if (!this.IsValid())
            {
                return 0;
            }
            return (int)this.pos;
        }

        public override int GetSize()
        {
            if (!this.IsValid())
            {
                return 0;
            }
            return (int)this.fileSize;
        }

        protected override void Dispose(bool is_release_managed_code)
        {
            if (this.is_disposed_)
            {
                return;
            }
            this.is_disposed_ = true;
        }
    }
}
