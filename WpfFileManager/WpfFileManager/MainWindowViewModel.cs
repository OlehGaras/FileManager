using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
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

        private readonly List<ShortcutAction> mFunctions = new List<ShortcutAction>();

        public List<ShortcutAction> GetActions()
        {
            return mFunctions;
        }

        public void AddAction(ShortcutAction action)
        {
            mFunctions.Add(action);
        }

        public MainWindowViewModel()
        {
            AddAction(new ShortcutAction("StyleChanged",ChangeStyle));
        }

        public event EventHandler AvailableFunctionsChanged;
        public event EventHandler<string> ShortcutPressed;

        protected virtual void OnShortcutPressed(string e)
        {
            var handler = ShortcutPressed;
            if (handler != null) handler(this, e);
        }

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
            OnAvailableFunctionsChanged();
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

        public void ChangeStyle()
        {
            OnStyleChanged();
        }

        public event EventHandler StyleChanged;

        protected virtual void OnStyleChanged()
        {
            EventHandler handler = StyleChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void SetStyle(string style)
        {
            throw new NotImplementedException();
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
            OnShortcutPressed(shortcutText.ToString());
        }

    }
}