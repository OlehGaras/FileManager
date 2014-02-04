using System.IO;
using System.Windows.Media;

namespace testPlugin
{
    public class FileInfo: IFileSystemInfo
    {
        public FileInfo(string path)
        {
            Path = path;
            DisplayName = new System.IO.FileInfo(path).Name;
        }
        public string Path { get; private set; }
        public string DisplayName { get; private set; }
        public ImageSource Icon { get;  set; }
    }
}