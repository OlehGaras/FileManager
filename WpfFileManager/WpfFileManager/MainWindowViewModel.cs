﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using FileManager;
using Xceed.Wpf.AvalonDock.Layout;
using Panel = FileManager.Panel;

namespace WpfFileManager
{
    [Export(typeof(IViewController))]
    [Export(typeof(ICurrentFileSystemState))]
    [Export(typeof(MainWindowViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainWindowViewModel : ViewModelBase, IViewController, ICurrentFileSystemState, IDisposable
    {
        private readonly IShortcutManager mShortcutManager;
        private readonly IErrorManager mErrorManager;
        private Version AppVersion { get { return new Version(1, 0); } }
        private readonly Dictionary<Guid, LayoutAnchorable> mToolsWindows = new Dictionary<Guid, LayoutAnchorable>();
        private readonly Dictionary<Guid, List<Guid>> mPluginsView = new Dictionary<Guid, List<Guid>>();

        [ImportingConstructor]
        public MainWindowViewModel(IShortcutManager shortcutManager, IErrorManager errorManager)
        {
            if (shortcutManager == null)
                throw new ArgumentNullException("shortcutManager");
            mShortcutManager = shortcutManager;

            if (errorManager == null)
                throw new ArgumentNullException("errorManager");
            mErrorManager = errorManager;
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
                    var pluginGuid = Guid.NewGuid();
                    plugin.Apply(pluginGuid);
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

        private ObservableCollection<LayoutAnchorable> mMenuItems = new ObservableCollection<LayoutAnchorable>();
        public ObservableCollection<LayoutAnchorable> MenuItems
        {
            get { return mMenuItems; }
            set
            {
                if (mMenuItems != value)
                {
                    mMenuItems = value;
                    OnPropertyChanged("MenuItems");
                }
            }
        }

        private ObservableCollection<LayoutAnchorable> mTools;
        public ObservableCollection<LayoutAnchorable> Tools
        {
            get { return mTools; }
            set
            {
                if (mTools != value)
                {
                    mTools = value;
                    OnPropertyChanged("Tools");
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

        public Guid AddToolPanel(Guid pluginGuid, UserControl content, string title)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (Tools != null && MenuItems != null)
            {
                var layoutAnchorable = new LayoutAnchorable
                    {
                        Content = content,
                        Title = title
                    };
                var guid = Guid.NewGuid();
                Tools.Add(layoutAnchorable);
                MenuItems.Add(layoutAnchorable);
                mToolsWindows.Add(guid, layoutAnchorable);

                if (mPluginsView.ContainsKey(pluginGuid))
                {
                    mPluginsView[pluginGuid].Add(guid);
                }
                else
                {
                    mPluginsView.Add(pluginGuid, new List<Guid> { guid });
                }

                return guid;
            }
            return default(Guid);
        }

        public void CloseToolPanel(Guid pluginGuid, Guid guid)
        {
            if (mToolsWindows.ContainsKey(guid) &&
                mPluginsView[pluginGuid].Contains(guid))
            {
                var anchorable = mToolsWindows[guid];
                mToolsWindows.Remove(guid);
                mPluginsView[pluginGuid].Remove(guid);
                if (Tools != null)
                {
                    Tools.Remove(anchorable);
                    MenuItems.Remove(anchorable);
                }
            }
        }

        //private DirectoryInfo mLeftCurrentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        private DirectoryInfo mLeftCurrentDirectory = new DirectoryInfo("D:\\");
        public DirectoryInfo LeftCurrentDirectory
        {
            get { return mLeftCurrentDirectory; }
            set
            {
                mLeftCurrentDirectory = value;
                OnCurrentDirectoryChanged();
            }
        }

        //private DirectoryInfo mRightCurrentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        private DirectoryInfo mRightCurrentDirectory = new DirectoryInfo("D:\\");
        public DirectoryInfo RightCurrentDirectory
        {
            get { return mRightCurrentDirectory; }
            set
            {
                mRightCurrentDirectory = value;
                OnCurrentDirectoryChanged();
            }
        }

        private IFileSystemInfo mRightSelectedItem;
        public IFileSystemInfo RightSelectedItem
        {
            get { return mRightSelectedItem = mRightSelectedItem ?? RightCurrentDirectory; }
            set
            {
                mRightSelectedItem = value;
                SetCurrentPanel(Panel.Right);
            }
        }

        public Panel CurrentPanel { get; private set; }

        private IFileSystemInfo mLefttSelectedItem;
        public IFileSystemInfo LeftSelectedItem
        {
            get { return mLefttSelectedItem = mLefttSelectedItem ?? LeftCurrentDirectory; }
            set
            {
                mLefttSelectedItem = value;
                SetCurrentPanel(Panel.Left);
            }
        }

        public void SetCurrentPanel(Panel panel)
        {
            CurrentPanel = panel;
        }

        public void SetLeftCurrentDirectory(DirectoryInfo dir)
        {
            LeftCurrentDirectory = dir;
        }

        public void SetRightCurrentDirectory(DirectoryInfo dir)
        {
            RightCurrentDirectory = dir;
        }

        public void SetLeftSelectedItem(IFileSystemInfo fileSystemInfo)
        {
            LeftSelectedItem = fileSystemInfo;
        }

        public void SetRightSelectedItem(IFileSystemInfo fileSystemInfo)
        {
            RightSelectedItem = fileSystemInfo;
        }

        private IList mLeftSelectedItems = new ObservableCollection<IFileSystemInfo>();
        public IList LeftSelectedItems
        {
            get { return mLeftSelectedItems; }
            set
            {
                if (mLeftSelectedItems != value)
                {
                    mLeftSelectedItems = value;
                }
                SetCurrentPanel(Panel.Left);
            }
        }

        private IList mRightSelectedItems = new ObservableCollection<IFileSystemInfo>();
        public IList RightSelectedItems
        {
            get { return mRightSelectedItems; }
            set
            {
                if (mRightSelectedItems != value)
                {
                    mRightSelectedItems = value;
                }
                SetCurrentPanel(Panel.Right);
            }
        }

        public void RefreshCurrentDirectory()
        {
            OnCurrentDirectoryChanged();
        }

        public void SetLeftSelectedItems(IList value)
        {
            LeftSelectedItems = value;
        }

        public void SetRightSelectedItems(IList value)
        {
            RightSelectedItems = value;
        }


        public event EventHandler CurrentDirectoryChanged;
        protected virtual void OnCurrentDirectoryChanged()
        {
            EventHandler handler = CurrentDirectoryChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private DelegateCommand mOnWindowKeyPressed;
        public ICommand OnWindowKeyPressed
        {
            get
            {
                if (mOnWindowKeyPressed == null)
                    mOnWindowKeyPressed = new DelegateCommand(KeyPressed);
                return mOnWindowKeyPressed;
            }

        }

        private void KeyPressed(object o)
        {
            var k = (KeyEventArgs)o;
            CatchShortcut(k);
        }

        public void CatchShortcut(KeyEventArgs k)
        {
            k.Handled = true;
            var key = (k.Key == Key.System ? k.SystemKey : k.Key);

            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            var shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());
            var s = shortcutText.ToString();
            var actionByKey = mShortcutManager.GetActionByKey(s);
            if (actionByKey != null)
            {
                actionByKey.Action();
                OnCurrentDirectoryChanged();
            }
        }

        private DelegateCommand mOnMenuItemPressed;
        public ICommand OnMenuItemPressed
        {
            get
            {
                if (mOnMenuItemPressed == null)
                {
                    mOnMenuItemPressed = new DelegateCommand(param => OpenView(param));
                }
                return mOnMenuItemPressed;
            }
        }

        private void OpenView(object o)
        {
            var layoutAnchorable = o as LayoutAnchorable;
            if (layoutAnchorable != null &&
                (mToolsWindows.ContainsValue(layoutAnchorable) &&
                !Tools.Contains(layoutAnchorable)))
            {
                Tools.Add(layoutAnchorable);
            }
        }

        public void Dispose()
        {
            var plugins = ServiceLocator.Current.GetAllInstances<IPlugin>();
            foreach (var plugin in plugins)
            {
                if (plugin.AppVersion == AppVersion)
                {
                    plugin.Dispose();
                }
            }
        }
    }
}