using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;
using FileManager;

namespace WpfFileManager
{
    [Export(typeof(IViewController))]
    [Export(typeof(ICurrentDirectory))]
    [Export(typeof(IShotcutManager))]
    [Export(typeof(MainWindowViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainWindowViewModel : ViewModelBase, IViewController, ICurrentDirectory ,IShotcutManager
    {
        private Version AppVersion { get { return new Version(1, 0); } }

        private List<Callback> mFunctions = new List<Callback>();
        public List<Callback> Functions
        {
            get { return mFunctions; }
            set
            {
                if (mFunctions != value)
                {
                    mFunctions = value;
                    OnPropertyChanged("Functions");
                }
            }
        }

        public void AddAction(Callback action)
        {
            Functions.Add(action);
            OnAvailableFunctionsChanged();
        }

        public event EventHandler AvailableFunctionsChanged;

        protected virtual void OnAvailableFunctionsChanged()
        {
            var handler = AvailableFunctionsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

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

        public void RegisterAvailableFunctions()
        {
            var plugins = ServiceLocator.Current.GetAllInstances<IPlugin>();
            foreach (var plugin in plugins)
            {
                if (plugin.AppVersion == AppVersion)
                {
                    //Functions.AddRange(plugin.RegisterAvailableFunctions());
                    OnAvailableFunctionsChanged();
                }
            }

        }

        private UserControl mShortCutPanel;
        public UserControl ShortCutPanel
        {
            get { return mShortCutPanel; }
            set
            {
                if (mShortCutPanel != value)
                {
                    mShortCutPanel = value;
                    OnPropertyChanged("ShortCutPanel");
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

        public void SetShortcutPanelContent(UserControl content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            ShortCutPanel = content;
        }

        private string mLeftCurrentDirectory = @"D:\";
        public string LeftCurrentDirectory
        {
            get { return mLeftCurrentDirectory; }
            set
            {
                mLeftCurrentDirectory = value;
                OnCurrentDirectoryChanged();
            }
        }

        private string mRightCurrentDirectory = @"D:\";
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

        private DelegateCommand mOpenShortcutsView;
        public ICommand OpenShortcutsView
        {
            get
            {
                if (mOpenShortcutsView == null)
                    mOpenShortcutsView = new DelegateCommand((param) => OpenView());
                return mOpenShortcutsView;
            }
        }

        private void OpenView()
        {
            throw new NotImplementedException();
        }
    }
}