using System;
using System.Globalization;
using System.Windows.Media;

namespace FileManager
{
    public class DirectoryInfo : IFileSystemInfo
    {
        public DirectoryInfo(string path)
        {
            var d = new System.IO.DirectoryInfo(path);
            Path = path;
            DisplayName = d.Name;
            Length = "Length: " + 0.ToString(CultureInfo.InvariantCulture);
            LastAccessTime = d.LastAccessTime;
            LastWritetime ="Last modified time: " +  d.LastWriteTime.ToString(CultureInfo.InvariantCulture);
            CreationTime = d.CreationTime;
            Extention = d.Extension;
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
