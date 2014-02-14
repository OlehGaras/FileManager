using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using FileManager;
using DirectoryInfo = FileManager.DirectoryInfo;
using FileInfo = FileManager.FileInfo;

namespace testPlugin
{
    public class DirectoryViewModel : ViewModelBase
    {
        private readonly ICurrentFileSystemState mCurrentFileSystemState;
        private readonly IErrorManager mErrorManager;
        private readonly Panel mPanel;
        private DirectoryInfo mCurrentPanelDirectory;

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
            var k = o as KeyEventArgs;
            var m = o as MouseEventArgs;
            if (k != null && k.Key == Key.Return)
            {
                if (SelectedItem is DirectoryInfo)
                    ChangeDirectory();
                if (SelectedItem is FileInfo)
                    System.Diagnostics.Process.Start(SelectedItem.Path);
            }
            if (m != null && m.LeftButton == MouseButtonState.Pressed)
            {
                if (SelectedItem is DirectoryInfo)
                    ChangeDirectory();
                if (SelectedItem is FileInfo)
                    System.Diagnostics.Process.Start(SelectedItem.Path);     
            }
        }

        private void ChangeDirectory()
        {
            var path = SelectedItem as DirectoryInfo;
            if (path == null)
                return;

            if (SelectedItem.GetType() == typeof(DoublePointInfo))
            {
                var directoryInfo = Directory.GetParent(SelectedItem.Path);
                if (directoryInfo != null)
                    path = new DirectoryInfo(directoryInfo.FullName);
                else
                    path = mCurrentPanelDirectory;
            }

            if (mPanel == Panel.Left)
            {
                mCurrentFileSystemState.SetLeftCurrentDirectory(path);
            }
            else
            {
                mCurrentFileSystemState.SetRightCurrentDirectory(path);
            }
        }

        public DirectoryViewModel(ICurrentFileSystemState currentFileSystemState, IErrorManager errorManager, Panel panel)
        {
            if (currentFileSystemState == null)
                throw new ArgumentNullException("currentFileSystemState");
            mCurrentFileSystemState = currentFileSystemState;
            mCurrentFileSystemState.CurrentDirectoryChanged += CurrentFileSystemStateOnCurrentFileSystemStateChanged;

            if (errorManager == null)
                throw new ArgumentNullException("errorManager");
            mErrorManager = errorManager;

            mPanel = panel;
            mCurrentPanelDirectory = mPanel == Panel.Left ? mCurrentFileSystemState.LeftCurrentDirectory : mCurrentFileSystemState.RightCurrentDirectory;
            LoadDirectory(mCurrentPanelDirectory);
        }

        private void CurrentFileSystemStateOnCurrentFileSystemStateChanged(object sender, EventArgs eventArgs)
        {
            //if (mPanel == Panel.Left && mCurrentPanelDirectory != mCurrentFileSystemState.LeftCurrentDirectory)
            if (mPanel == Panel.Left)
            {
                //mCurrentPanelDirectory = mCurrentFileSystemState.LeftCurrentDirectory;
                LoadDirectory(mCurrentFileSystemState.LeftCurrentDirectory);
            }

            //if (mPanel == Panel.Right && mCurrentPanelDirectory != mCurrentFileSystemState.RightCurrentDirectory)
            if (mPanel == Panel.Right)
            {
                //mCurrentPanelDirectory = mCurrentFileSystemState.RightCurrentDirectory;
                LoadDirectory(mCurrentFileSystemState.RightCurrentDirectory);
            }
        }

        private void LoadDirectory(DirectoryInfo currentDirectory)
        {
            try
            {
                var directories = Directory.EnumerateDirectories(currentDirectory.Path);
                var files = Directory.EnumerateFiles(currentDirectory.Path);

                var fileSystemInfos =
                    directories.Select(d => new DirectoryInfo(d))
                               .Cast<IFileSystemInfo>()
                               .Concat(files.Select(f => new FileInfo(f)))
                               .ToList();
                SetTheIcon(fileSystemInfos);
                fileSystemInfos.Insert(0, new DoublePointInfo(currentDirectory.Path));
                FileSystemInfo = new ObservableCollection<IFileSystemInfo>(fileSystemInfos);
                mCurrentPanelDirectory = currentDirectory;
            }
            catch (Exception e)
            {
                mErrorManager.AddError(new Error(e));
            }
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
                    if (mPanel == Panel.Left)
                    {
                        mCurrentFileSystemState.SetLeftSelectedItem(value);
                    }
                    else
                    {
                        mCurrentFileSystemState.SetRightSelectedItem(value);
                    }

                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        private string mStyle = "Style1";
        public string Style
        {
            get { return mStyle; }
            set
            {
                if (mStyle != value)
                {
                    mStyle = value;
                    OnPropertyChanged("Style");
                }
            }
        }

        public void ChangeStyle()
        {
            mStyle = Style == "Style1" ? "Style2" : "Style1";
            OnPropertyChanged("Style");
        }

        public void SetStyle(string style)
        {
            Style = style;
        }
    }
}