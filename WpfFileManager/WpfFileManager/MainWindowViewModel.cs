using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using FileManager;

namespace WpfFileManager
{
    [Export(typeof(IViewController))]
    [Export(typeof(ICurrentDirectory))]
    [Export(typeof(MainWindowViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainWindowViewModel : ViewModelBase, IViewController, ICurrentDirectory
    {
        private Version AppVersion { get { return new Version(1, 0); } }

        private UserControl mLeftPanel;
        public UserControl LeftPanel
        {
            get { return mLeftPanel; }
            set
            {
                if (mLeftPanel != value)
                {
                    mLeftPanel = value;
                    OnPropertyChanged("LeftPanel");
                }
            }
        }

        private UserControl mRightPanel;
        public UserControl RightPanel
        {
            get { return mRightPanel; }
            set
            {
                if (mRightPanel != value)
                {
                    mRightPanel = value;
                    OnPropertyChanged("RightPanel");
                }
            }
        }

        public void ApplyPlugins()
        {
            var plugins = ServiceLocator.Current.GetAllInstances<IPlugin>();
            foreach (var plugin in plugins)
            {
                if (plugin.AppVersion == AppVersion)
                {
                    plugin.Apply();
                }
            }
        }


        public void SetLeftPanelContent(UserControl content)
        {
            if (content == null)
                throw new ArgumentNullException("content");
            LeftPanel = content;
        }

        public void SetRightPanelContent(UserControl content)
        {
            if (content == null)
                throw new ArgumentNullException("content");
            RightPanel = content;
        }

        private string mLeftCurrentDirectory = @"D:\repos\SameFilesFinder";
        public string LeftCurrentDirectory
        {
            get { return mLeftCurrentDirectory; }
            set
            {
                mLeftCurrentDirectory = value;
                OnCurrentDirectoryChanged();
            }
        }

        private string mRightCurrentDirectory = @"D:\repos\SameFilesFinder";
        public string RightCurrentDirectory
        {
            get { return mRightCurrentDirectory; }
            set
            {
                mRightCurrentDirectory = value;
                OnCurrentDirectoryChanged();
            }
        }
        public void SetLeftCurrentDirectory(string dir)
        {
            LeftCurrentDirectory = dir;
        }

        public void SetRightCurrentDirectory(string dir)
        {
            RightCurrentDirectory = dir;
        }

        public event EventHandler CurrentDirectoryChanged;
        protected virtual void OnCurrentDirectoryChanged()
        {
            EventHandler handler = CurrentDirectoryChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}