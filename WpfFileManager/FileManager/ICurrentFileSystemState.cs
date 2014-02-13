using System;

namespace FileManager
{
    public interface ICurrentFileSystemState
    {
        DirectoryInfo LeftCurrentDirectory { get; }
        DirectoryInfo RightCurrentDirectory { get; }
        IFileSystemInfo LeftSelectedItem { get; }
        IFileSystemInfo RightSelectedItem { get; }
        Panel CurrentPanel { get; }
        void SetCurrentPanel(Panel panel);
        void SetLeftCurrentDirectory(DirectoryInfo dir);
        void SetRightCurrentDirectory(DirectoryInfo dir);
        void SetLeftSelectedItem(IFileSystemInfo fileSystemInfo);
        void SetRightSelectedItem(IFileSystemInfo fileSystemInfo);
        event EventHandler CurrentDirectoryChanged;
    }
}