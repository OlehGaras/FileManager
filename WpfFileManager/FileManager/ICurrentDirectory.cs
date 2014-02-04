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

    public class UpCurrentDirrectory: ICurrentDirectory
    {
        public string LeftCurrentDirectory { get; private set; }
        public string RightCurrentDirectory { get; private set; }
        public void SetLeftCurrentDirectory(string dir)
        {
            throw new NotImplementedException();
        }

        public void SetRightCurrentDirectory(string dir)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CurrentDirectoryChanged;

        protected virtual void OnCurrentDirectoryChanged()
        {
            var handler = CurrentDirectoryChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}