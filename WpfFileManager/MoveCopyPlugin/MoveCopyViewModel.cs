using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using FileManager;
using DirectoryInfo = FileManager.DirectoryInfo;
using FileInfo = FileManager.FileInfo;

namespace MoveCopyPlugin
{
    public class MoveCopyViewModel : ViewModelBase
    {
        private readonly ICurrentFileSystemState mCurrentFileSystemState;
        private readonly IErrorManager mErrorManager;

        public  BackgroundWorker BackgroundWorker = new BackgroundWorker();
        private CancellationTokenSource mCts;

        private Visibility mVisible = Visibility.Collapsed;
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

            //mCts = new CancellationTokenSource();
            ////BackgroundWorker.DoWork += GetGroupsOfFiles;
            //BackgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
            //BackgroundWorker.WorkerSupportsCancellation = true;
            //BackgroundWorker.WorkerReportsProgress = true;
        }

        public void Copy(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            Visible = Visibility.Visible;
            var fileSystemInfo = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItem
                                                 : mCurrentFileSystemState.RightSelectedItem;
            DirectoryInfo targetDir = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                          ? mCurrentFileSystemState.RightCurrentDirectory
                                          : mCurrentFileSystemState.LeftCurrentDirectory;
            
            CopyImpl(fileSystemInfo, targetDir);
            Visible = Visibility.Collapsed;
        }

        private void CopyImpl(IFileSystemInfo fileSystemInfo, DirectoryInfo targetDir)
        {
            try
            {
                var destDirName = Path.Combine(targetDir.Path, fileSystemInfo.DisplayName);
                if (fileSystemInfo is FileInfo)
                {
                    File.Copy(fileSystemInfo.Path, destDirName);
                }
                if (fileSystemInfo is DirectoryInfo)
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

            if (!dir.Exists)
            {
                mErrorManager.AddError(new Error(
                    new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName)));
                return;
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }
                catch (Exception exception)
                {
                    mErrorManager.AddError(new Error(exception));
                }
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public void Move()
        {
            Visible = Visibility.Visible;
            IFileSystemInfo fileSystemInfo = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItem
                                                 : mCurrentFileSystemState.RightSelectedItem;
            DirectoryInfo targetDir = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                          ? mCurrentFileSystemState.RightCurrentDirectory
                                          : mCurrentFileSystemState.LeftCurrentDirectory;
            MoveImpl(fileSystemInfo, targetDir);
            Visible = Visibility.Collapsed;
        }

        private void MoveImpl(IFileSystemInfo fileSystemInfo, DirectoryInfo targetDir)
        {
            try
            {
                var destDirName = Path.Combine(targetDir.Path, fileSystemInfo.DisplayName);
                if (fileSystemInfo is FileInfo)
                {
                    File.Move(fileSystemInfo.Path, destDirName);
                }
                if (fileSystemInfo is DirectoryInfo)
                {
                    Directory.Move(fileSystemInfo.Path, destDirName);
                }
            }
            catch (Exception exception)
            {
                mErrorManager.AddError(new Error(exception));
            }
        }

        public void Delete()
        {
            Visible = Visibility.Visible;
            IFileSystemInfo fileSystemInfo = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItem
                                                 : mCurrentFileSystemState.RightSelectedItem;
            DeleteImpl(fileSystemInfo);
            Visible = Visibility.Collapsed;
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
