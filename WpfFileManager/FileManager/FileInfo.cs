using System;
using System.Globalization;
using System.Windows.Media;

namespace FileManager
{
    public class FileInfo : IFileSystemInfo
    {
        public FileInfo(string path)
        {
            Path = path;
            var f = new System.IO.FileInfo(path);
            DisplayName = f.Name;
            Extention = "Extention: " + f.Extension;
            LastAccessTime = f.LastAccessTime;
            LastWritetime ="Last modified time: " + f.LastWriteTime.ToString(CultureInfo.InvariantCulture);
            CreationTime = f.CreationTime;
            Length ="Length: " + f.Length.ToString(CultureInfo.InvariantCulture);
        }
        public string Path { get; private set; }
        public string DisplayName { get; private set; }
        public ImageSource Icon { get; set; }
        public string Extention { get; private set; }
        public string LastWritetime { get; private set; }
        public DateTime LastAccessTime { get; private set; }
        public DateTime CreationTime { get; private set; }
        public string Length { get; private set; }
    }
}
