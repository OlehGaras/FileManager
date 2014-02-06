using System;
using System.Windows.Media;

namespace testPlugin
{
    public interface IFileSystemInfo
    {
        string Path { get; }
        string DisplayName { get; }
        ImageSource Icon { get; set; }
        string Extention { get;  }
        DateTime LastWritetime { get;  }
        DateTime LastAccessTime { get;  }
        DateTime CreationTime { get;  }
        long Length { get; }
    }
}