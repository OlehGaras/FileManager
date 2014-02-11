using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using FileManager;
using Xceed.Wpf.AvalonDock.Layout;

namespace WpfFileManager
{
    [Export(typeof(IViewController))]
    [Export(typeof(ICurrentDirectory))]
    [Export(typeof(MainWindowViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainWindowViewModel : ViewModelBase, IViewController, ICurrentDirectory
    {
        private readonly IShortcutManager mShortcutManager;
        private Version AppVersion { get { return new Version(1, 0); } }
        private Dictionary<Guid, LayoutAnchorable> mToolsWindows = new Dictionary<Guid, LayoutAnchorable>();
            
        [ImportingConstructor]
        public MainWindowViewModel(IShortcutManager shortcutManager)
        {
            if (shortcutManager == null) 
                throw new ArgumentNullException("shortcutManager");
            mShortcutManager = shortcutManager;
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

        public Guid AddToolPanel(UserControl content, string title)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (Tools != null)
            {
                var layoutAnchorable = new LayoutAnchorable
                    {
                        Content = content,
                        Title = title
                    };

                var guid = Guid.NewGuid();
                Tools.Add(layoutAnchorable);
                mToolsWindows.Add(guid, layoutAnchorable);
                return guid;
            }
            return default(Guid);
        }

        public void CloseToolPanel(Guid guid)
        {
            if (mToolsWindows.ContainsKey(guid))
            {
                var anchorable = mToolsWindows[guid];
                mToolsWindows.Remove(guid);
                if (Tools != null)
                    Tools.Remove(anchorable);
            }
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
                    mOpenShortcutsView = new DelegateCommand(param => OpenView());
                return mOpenShortcutsView;
            }
        }

        private void OpenView()
        {
            throw new Exception();
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
            }
        }
    }
}