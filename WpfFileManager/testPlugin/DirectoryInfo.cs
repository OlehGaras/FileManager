using System.IO;
using System.Windows.Media;

namespace testPlugin
{
    public class DirectoryInfo : IFileSystemInfo
    {
        public DirectoryInfo(string path)
        {
            Path = path;
            DisplayName = new System.IO.DirectoryInfo(path).Name;
        }
        public string Path { get; private set; }
        public string DisplayName { get; private set; }
        public ImageSource Icon { get;  set; }
    }

    public class DoublePointInfo : DirectoryInfo
    {
        public DoublePointInfo(string path):base(path)
        {
        }
    }
}