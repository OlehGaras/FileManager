using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using FileManager;

namespace testPlugin
{
    public enum Panel
    {
        Left,
        Right
    }

    public class DirectoryViewModel : ViewModelBase
    {
        private readonly ICurrentDirectory mCurrentDirectory;
        private readonly Panel mPanel;
        private string mCurrentPanelDirectory;

        private DelegateCommand mOnListItemKeyPressed;

        public ICommand OnListItemKeyPressed
        {
            get
            {
                if (mOnListItemKeyPressed == null)
                    mOnListItemKeyPressed = new DelegateCommand(KeyPressed);

                return mOnListItemKeyPressed;
            }

        }

        private void KeyPressed(object o)
        {
            var k = (KeyEventArgs)o;
            if (k.Key == Key.Return && SelectedItem is DirectoryInfo)
            {
                var path = SelectedItem.Path;
                if (SelectedItem.GetType() == typeof (DoublePointInfo))
                {
                    var directoryInfo = Directory.GetParent(SelectedItem.Path);
                    if (directoryInfo != null)
                        path = directoryInfo.FullName;
                    else
                        path = mCurrentPanelDirectory;
                }

                if (mPanel == Panel.Left)
                {
                    mCurrentDirectory.SetLeftCurrentDirectory(path);
                }
                else
                {
                    mCurrentDirectory.SetRightCurrentDirectory(path);
                }
            }
        }

        public DirectoryViewModel(ICurrentDirectory currentDirectory, Panel panel)
        {
            if (currentDirectory == null)
                throw new ArgumentNullException("currentDirectory");
            mCurrentDirectory = currentDirectory;
            mCurrentDirectory.CurrentDirectoryChanged += CurrentDirectoryOnCurrentDirectoryChanged;
            mPanel = panel;
            mCurrentPanelDirectory = mPanel == Panel.Left ? mCurrentDirectory.LeftCurrentDirectory : mCurrentDirectory.RightCurrentDirectory;
            LoadDirectory(mCurrentPanelDirectory);
        }

        private void CurrentDirectoryOnCurrentDirectoryChanged(object sender, EventArgs eventArgs)
        {
            if (mPanel == Panel.Left && string.Compare(mCurrentPanelDirectory, mCurrentDirectory.LeftCurrentDirectory, StringComparison.OrdinalIgnoreCase) != 0)
            {
                mCurrentPanelDirectory = mCurrentDirectory.LeftCurrentDirectory;
                LoadDirectory(mCurrentPanelDirectory);
            }

            if (mPanel == Panel.Right && string.Compare(mCurrentPanelDirectory, mCurrentDirectory.RightCurrentDirectory, StringComparison.OrdinalIgnoreCase) != 0)
            {
                mCurrentPanelDirectory = mCurrentDirectory.RightCurrentDirectory;
                LoadDirectory(mCurrentPanelDirectory);
            }
        }

        private void LoadDirectory(string currentDirectory)
        {
            var directories = Directory.EnumerateDirectories(currentDirectory);
            var files = Directory.EnumerateFiles(currentDirectory);

            var fileSystemInfos =
                directories.Select(d => new DirectoryInfo(d))
                           .Cast<IFileSystemInfo>()
                           .Concat(files.Select(f => new FileInfo(f)))
                           .ToList();
            SetTheIcon(fileSystemInfos);
            fileSystemInfos.Insert(0, new DoublePointInfo(currentDirectory));
            FileSystemInfo = new ObservableCollection<IFileSystemInfo>(fileSystemInfos);
        }

        private void SetTheIcon(IEnumerable<IFileSystemInfo> fileSystemInfos)
        {
            foreach (var fileSystemInfo in fileSystemInfos)
            {
                if (fileSystemInfo is DirectoryInfo)
                {
                    fileSystemInfo.Icon = FolderManager.GetImageSource(fileSystemInfo.Path, ShellManager.ItemState.Undefined);
                }
                else
                {
                    fileSystemInfo.Icon = FileManager.GetImageSource(fileSystemInfo.Path);
                }
            }
        }

        private ObservableCollection<IFileSystemInfo> mFileSystemInfos;
        public ObservableCollection<IFileSystemInfo> FileSystemInfo
        {
            get { return mFileSystemInfos; }
            set
            {
                if (mFileSystemInfos != value)
                {
                    mFileSystemInfos = value;
                    OnPropertyChanged("FileSystemInfo");
                }
            }
        }

        private IFileSystemInfo mSelectedItem;
        public IFileSystemInfo SelectedItem
        {
            get { return mSelectedItem; }
            set
            {
                if (mSelectedItem != value)
                {
                    mSelectedItem = value;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }
    }
}