using System;

namespace FileManager
{
    public interface ICurrentDirectory
    {
        string LeftCurrentDirectory { get; }
        string RightCurrentDirectory { get; }
        void SetLeftCurrentDirectory(string dir);
        void SetRightCurrentDirectory(string dir);
        event EventHandler CurrentDirectoryChanged;
    }
}