using System;
using System.Windows.Media;

namespace FileManager
{
    public interface IFileSystemInfo
    {
        string Path { get; }
        string DisplayName { get; }
        ImageSource Icon { get; set; }
        string Extention { get; }
        string LastWritetime { get; }
        DateTime LastAccessTime { get; }
        DateTime CreationTime { get; }
        string Length { get; }
    }
}
