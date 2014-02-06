using System;
using System.IO;
using System.Windows.Media;

namespace testPlugin
{
    public class FileInfo: IFileSystemInfo
    {
        public FileInfo(string path)
        {
            Path = path;
            var f = new System.IO.FileInfo(path);
            DisplayName = f.Name;
            Extention = f.Extension;
            LastAccessTime = f.LastAccessTime;
            LastWritetime = f.LastWriteTime;
            CreationTime = f.CreationTime;
            Length = f.Length;
        }
        public string Path { get; private set; }
        public string DisplayName { get; private set; }
        public ImageSource Icon { get;  set; }
        public string Extention { get; private set; }
        public DateTime LastWritetime { get; private set; }
        public DateTime LastAccessTime { get; private set; }
        public DateTime CreationTime { get; private set; }
        public long Length { get; private set; }
    }
}