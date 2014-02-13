using System;
using System.IO;
using FileManager;
using DirectoryInfo = FileManager.DirectoryInfo;
using FileInfo = FileManager.FileInfo;

namespace MoveCopyPlugin
{
    public class MoveCopyViewModel : ViewModelBase
    {
        private readonly ICurrentFileSystemState mCurrentFileSystemState;
        private readonly IErrorManager mErrorManager;

        public MoveCopyViewModel(ICurrentFileSystemState currentFileSystemState, IErrorManager errorManager)
        {
            if (currentFileSystemState == null)
                throw new ArgumentNullException("currentFileSystemState");
            if (errorManager == null)
                throw new ArgumentNullException("errorManager");
            mCurrentFileSystemState = currentFileSystemState;
            mErrorManager = errorManager;
        }

        public void Copy()
        {
            IFileSystemInfo fileSystemInfo = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItem
                                                 : mCurrentFileSystemState.RightSelectedItem;
            DirectoryInfo targetDir = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                          ? mCurrentFileSystemState.RightCurrentDirectory
                                          : mCurrentFileSystemState.LeftCurrentDirectory;

            CopyImpl(fileSystemInfo, targetDir);

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
            IFileSystemInfo fileSystemInfo = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItem
                                                 : mCurrentFileSystemState.RightSelectedItem;
            DirectoryInfo targetDir = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                          ? mCurrentFileSystemState.RightCurrentDirectory
                                          : mCurrentFileSystemState.LeftCurrentDirectory;
            MoveImpl(fileSystemInfo, targetDir);
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
            IFileSystemInfo fileSystemInfo = mCurrentFileSystemState.CurrentPanel == Panel.Left
                                                 ? mCurrentFileSystemState.LeftSelectedItem
                                                 : mCurrentFileSystemState.RightSelectedItem;
            DeleteImpl(fileSystemInfo);
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
}
