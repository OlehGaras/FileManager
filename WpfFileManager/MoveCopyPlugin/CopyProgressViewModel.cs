using System;
using FileManager;

namespace MoveCopyPlugin
{
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
}