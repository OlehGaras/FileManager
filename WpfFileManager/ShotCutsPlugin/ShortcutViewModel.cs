using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Input;
using FileManager;

namespace ShotCutsPlugin
{
    public class ShortcutViewModel : ViewModelBase
    {
        private readonly IShotcutManager mShotcutManager;
        private List<ShortcutAction> mCallbacks = new List<ShortcutAction>();
        public List<ShortcutAction> Callbacks
        {
            get { return mCallbacks; }
            set
            {
                if (mCallbacks != value)
                {
                    mCallbacks = value;
                    OnPropertyChanged("Callbacks");
                }
            }
        }

        public ShortcutViewModel(IShotcutManager shotcutManager)
        {
            mShotcutManager = shotcutManager;
            mShotcutManager.AvailableFunctionsChanged +=
                (sender, args) =>
                    {
                        mCallbacks.AddRange(mShotcutManager.GetActions());
                        OnPropertyChanged("CallBacks");
                        Deserialize();
                    };
            mShotcutManager.ShortcutPressed += (sender, s) =>
                {
                    var action = (from shortcut in Shortcuts where shortcut.ShortcutText == s select shortcut.CurrCallback).FirstOrDefault();
                    if (action != null) action.Action();
                };
        }

        private void Serialize()
        {
            var file = new FileStream("Shotcuts.txt", FileMode.Create);
            var serializer = new JavaScriptSerializer() ;
            var json = serializer.Serialize(mShortcuts);
            var writer = new StreamWriter(file) { AutoFlush = true};
            writer.WriteLine(json);
            file.Close();
        }

        private void Deserialize()
        {
            var file = new FileStream("Shotcuts.txt",FileMode.Open);
            try
            {
                var reader = new StreamReader(file);
                var json = reader.ReadToEnd();
                var deserializer = new JavaScriptSerializer();
                var shortcuts =(ObservableCollection<Shortcut>)deserializer.Deserialize(json, typeof(ObservableCollection<Shortcut>));
                foreach (var shortcut in shortcuts)
                {
                    var i = Callbacks.IndexOf(Callbacks.Find(c => c.Name == shortcut.CurrCallback.Name));
                    shortcut.CurrCallback = Callbacks[i];
                    shortcut.Functions = Callbacks;
                }
                Shortcuts = shortcuts;
                file.Close();
            }
            catch (Exception e)
            {
                file.Close();
            }
            
        }

        private ObservableCollection<Shortcut> mShortcuts = new ObservableCollection<Shortcut>();
        public ObservableCollection<Shortcut> Shortcuts
        {
            get { return mShortcuts; }
            set
            {
                if (mShortcuts != value)
                {
                    mShortcuts = value;
                    OnPropertyChanged("Shortcuts");
                }
            }
        }

        private string mNewShortcutText;
        public string NewShortcutText
        {
            get { return mNewShortcutText; }
            set
            {
                if (mNewShortcutText != value)
                {
                    mNewShortcutText = value;
                    OnPropertyChanged("NewShortcutText");
                }
            }
        }

        private Shortcut mListSelectedItem;
        public Shortcut ListSelectedItem
        {
            get { return mListSelectedItem; }
            set
            {
                if (mListSelectedItem != value)
                {
                    mListSelectedItem = value;

                    OnPropertyChanged("ListSelectedItem");
                }
            }
        }

        private ShortcutAction mComboSelectedItem;
        public ShortcutAction ComboSelectedItem
        {
            get { return mComboSelectedItem; }
            set
            {
                if (mComboSelectedItem != value)
                {
                    mComboSelectedItem = value;

                    OnPropertyChanged("ComboSelectedItem");
                }
            }
        }

        private DelegateCommand mOnListItemKeyPressed;
        public ICommand OnListItemKeyPressed
        {
            get
            {
                if (mOnListItemKeyPressed == null)
                    mOnListItemKeyPressed = new DelegateCommand(KeyPressed);
                return mOnListItemKeyPressed;
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
            NewShortcutText = shortcutText.ToString();
        }

        private DelegateCommand mAddNewShortcut;
        public ICommand AddNewShortcut
        {
            get
            {
                if (mAddNewShortcut == null)
                {
                    mAddNewShortcut = new DelegateCommand(param=>AddShortcut());
                }
                return mAddNewShortcut;
            }
        }

        private void AddShortcut()
        {
            if (NewShortcutText != null && ComboSelectedItem != null && Callbacks != null)
                mShortcuts.Add(new Shortcut(NewShortcutText,ComboSelectedItem,Callbacks));
            OnPropertyChanged("Shortcuts");
        }

        private DelegateCommand mSaveShortcuts;
        public ICommand SaveShortcuts
        {
            get
            {
                if (mSaveShortcuts == null)
                {
                    mSaveShortcuts = new DelegateCommand(param=> Serialize());
                }
                return mSaveShortcuts;
            }
        }

        private DelegateCommand mDeleteShortcut;
        public ICommand DeleteShortcut
        {
            get
            {
                if (mDeleteShortcut == null)
                {
                    mDeleteShortcut = new DelegateCommand(param =>DeleteShC());
                }
                return mDeleteShortcut;
            }
        }

        private void DeleteShC()
        {
            mShortcuts.Remove(ListSelectedItem);
            OnPropertyChanged("Shortcuts");
        }
    }

}
