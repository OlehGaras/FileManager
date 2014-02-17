using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace FileManager
{
    public interface ICurrentFileSystemState
    {
        DirectoryInfo LeftCurrentDirectory { get; }
        DirectoryInfo RightCurrentDirectory { get; }
        IFileSystemInfo LeftSelectedItem { get; }
        IFileSystemInfo RightSelectedItem { get; }
        IList LeftSelectedItems { get; }
        IList RightSelectedItems { get; }
        Panel CurrentPanel { get; }
        void SetCurrentPanel(Panel panel);
        void SetLeftCurrentDirectory(DirectoryInfo dir);
        void SetRightCurrentDirectory(DirectoryInfo dir);
        void SetLeftSelectedItem(IFileSystemInfo fileSystemInfo);
        void SetRightSelectedItem(IFileSystemInfo fileSystemInfo);
        event EventHandler CurrentDirectoryChanged;
        void RefreshCurrentDirectory();
        void SetLeftSelectedItems(IList value);
        void SetRightSelectedItems(IList value);
    }
}