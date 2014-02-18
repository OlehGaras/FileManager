using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using FileManager;
using DirectoryInfo = FileManager.DirectoryInfo;
using FileInfo = FileManager.FileInfo;
using System.Linq;

namespace MoveCopyPlugin
{
    public class MoveCopyViewModel : ViewModelBase
    {
        private readonly ICurrentFileSystemState mCurrentFileSystemState;
        private readonly IErrorManager mErrorManager;
        private CopyProgressViewModel mCurrentFile;

        public BackgroundWorker BackgroundWorker = new BackgroundWorker();
        private CancellationTokenSource mCts;

        private CopyProgressViewModel[] mFiles;
        public CopyProgressViewModel[] Files
        {
            get { return mFiles; }
            set
            {
                if (value != mFiles)
                {
                    mFiles = value;
                    OnPropertyChanged("Files");
                }
            }
        }

        private Visibility mVisible = Visibility.Collapsed;
        private Dispatcher mDispatcher;

        public Visibility Visible
        {
            get { return mVisible; }
            set
            {
                if (mVisible != value)
                {
                    mVisible = value;
                    OnPropertyChanged("Visible");
                }
            }
        }

        public MoveCopyViewModel(ICurrentFileSystemState currentFileSystemState, IErrorManager errorManager)
        {
            if (currentFileSystemState == null)
                throw new ArgumentNullException("currentFileSystemState");
            if (errorManager == null)
                throw new ArgumentNullException("errorManager");
            mCurrentFileSystemState = currentFileSystemState;
            mErrorManager = errorManager;
            mDispatcher = Dispatcher.CurrentDispatcher;
            mCts = new CancellationTokenSource();
            InitializeWorker();
        }

        public void InitializeWorker()
        {
            BackgroundWorker = new BackgroundWorker();
            BackgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
            BackgroundWorker.WorkerSupportsCancellation = true;
            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.ProgressChanged += (sender, args) =>
                {
                    if (mCurrentFile != null)
                    {
                        var progressPercentage = args.ProgressPercentage;
                        mDispatcher.Invoke(() => mCurrentFile.Progress = progressPercentage);
                    }
                };
        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Visible = Visibility.Collapsed;
            mCurrentFileSystemState.SetCurrentPanel(Panel.Left);
            mCurrentFileSystemState.RefreshCurrentDirectory();
            mCurrentFileSystemState.SetCurrentPanel(Panel.Right);
            mCurrentFileSystemState.RefreshCurrentDirectory();
        }

        public void Copy(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var fileSystemInfos = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItems
                                                 : mCurrentFileSystemState.RightSelectedItems;

            var arr = new IFileSystemInfo[fileSystemInfos.Count];
            fileSystemInfos.CopyTo(arr, 0);

            Files = arr.Select(item => new CopyProgressViewModel(item)).ToArray();

            var targetDir = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                          ? mCurrentFileSystemState.RightCurrentDirectory
                                          : mCurrentFileSystemState.LeftCurrentDirectory;

            foreach (var v in Files)
            {
                CopyImpl(v, targetDir);
            }
        }

        private void CopyImpl(CopyProgressViewModel copyProgressViewModel, DirectoryInfo targetDir)
        {
            try
            {
                mCurrentFile = copyProgressViewModel;
                var fileSystemInfo = copyProgressViewModel.FileSystemInfo;
                var destDirName = Path.Combine(targetDir.Path, fileSystemInfo.DisplayName);
                var fc = new FileCopy(1);

                fc.Progress += (sender, i) =>
                    {
                        if (BackgroundWorker != null)
                            BackgroundWorker.ReportProgress((int)i);
                    };

                if (copyProgressViewModel.IsFile)
                {
                    fc.Copy(fileSystemInfo.Path, destDirName);
                    BackgroundWorker.ReportProgress(100);                  
                }

                if (copyProgressViewModel.IsDir)
                {
                    DirectoryCopy(fileSystemInfo.Path, destDirName, true);
                }
            }
            catch (Exception exception)
            {
                mErrorManager.AddError(new Error(exception));
            }
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new System.IO.DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();
            var fileCopy = new FileCopy(4);
            fileCopy.Progress += (sender, i) =>
            {
                if (BackgroundWorker != null)
                    BackgroundWorker.ReportProgress((int)i);
            };

            if (!dir.Exists)
            {
                mErrorManager.AddError(new Error(
                    new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName)));
                return;
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                try
                {               
                    var temppath = Path.Combine(destDirName, file.Name);
                    fileCopy.Copy(file.FullName, temppath);
                }
                catch (Exception exception)
                {
                    mErrorManager.AddError(new Error(exception));
                }
            }

            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public void Move(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var fileSystemInfos = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItems
                                                 : mCurrentFileSystemState.RightSelectedItems;
            var targetDir = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                          ? mCurrentFileSystemState.RightCurrentDirectory
                                          : mCurrentFileSystemState.LeftCurrentDirectory;

            var arr = new IFileSystemInfo[fileSystemInfos.Count];
            fileSystemInfos.CopyTo(arr, 0);

            Files = arr.Select(item => new CopyProgressViewModel(item)).ToArray();

            foreach (var v in Files)
            {
                CopyImpl(v,targetDir);
                //MoveImpl(v, targetDir);
            }
            foreach (var v in Files)
            {
                DeleteImpl(v.FileSystemInfo);
            }
        }

        private void MoveImpl(CopyProgressViewModel copyProgressViewModel, DirectoryInfo targetDir)
        {
            try
            {
                var fileSystemInfo = copyProgressViewModel.FileSystemInfo;
                var destDirName = Path.Combine(targetDir.Path, fileSystemInfo.DisplayName);
                var fc = new FileCopy(1);

                fc.Progress += (sender, i) =>
                {
                    if (BackgroundWorker != null)
                        BackgroundWorker.ReportProgress((int)i);
                };

                if (copyProgressViewModel.IsFile)
                {
                    File.Move(fileSystemInfo.Path, destDirName);
                }
                if (copyProgressViewModel.IsDir)
                {
                    Directory.Move(fileSystemInfo.Path, destDirName);
                }
            }
            catch (Exception exception)
            {
                mErrorManager.AddError(new Error(exception));
            }
        }

        public void Delete(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            Visible = Visibility.Visible;
            var fileSystemInfos = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItems
                                                 : mCurrentFileSystemState.RightSelectedItems;

            var arr = new IFileSystemInfo[fileSystemInfos.Count];
            fileSystemInfos.CopyTo(arr, 0);

            foreach (var v in arr)
            {
                DeleteImpl(v);
            }
        }

        private void DeleteImpl(IFileSystemInfo fileSystemInfo)
        {
            try
            {
                if (fileSystemInfo is FileInfo)
                {
                    File.Delete(fileSystemInfo.Path);
                }
                if (fileSystemInfo is DirectoryInfo)
                {
                    Directory.Delete(fileSystemInfo.Path, true);
                }
            }
            catch (Exception exception)
            {
                mErrorManager.AddError(new Error(exception));
            }
        }
    }

    public class CopyProgressViewModel : ViewModelBase
    {
        public readonly IFileSystemInfo FileSystemInfo;

        public CopyProgressViewModel(IFileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo == null)
                throw new ArgumentNullException("fileSystemInfo");
            FileSystemInfo = fileSystemInfo;
        }

        public bool IsFile { get { return FileSystemInfo is FileInfo; } }
        public bool IsDir { get { return FileSystemInfo is DirectoryInfo; } }

        private int mProgress;
        public int Progress
        {
            get { return mProgress; }
            set
            {
                if (mProgress != value)
                {
                    mProgress = value;
                    OnPropertyChanged("Progress");
                }
            }
        }

        public string DisplayName
        {
            get { return FileSystemInfo.DisplayName; }
            set
            {
                if (DisplayName != value)
                {
                    DisplayName = value;
                    OnPropertyChanged("DisplayName");
                }
            }
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
