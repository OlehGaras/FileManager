using System.Windows.Media;

namespace testPlugin
{
    public interface IFileSystemInfo
    {
        string Path { get; }
        string DisplayName { get; }
        ImageSource Icon { get; set; }
    }
}