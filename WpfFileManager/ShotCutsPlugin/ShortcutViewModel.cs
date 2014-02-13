using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Input;
using FileManager;
using System.Linq;

namespace ShotCutsPlugin
{
    public class ShortcutViewModel : ViewModelBase
    {
        private readonly IShortcutManager mShortcutManager;
        private ObservableCollection<Shortcut> mDeserializedShortcuts;

        public List<ShortcutAction> Callbacks
        {
            get { return mShortcutManager.GetActions(); }
        }

        public ShortcutViewModel(IShortcutManager shortcutManager)
        {
            mShortcutManager = shortcutManager;
            shortcutManager.ActionsChanged += ShortcutManagerOnActionsChanged;
            Deserialize();
            ReloadShortcutsMapping();
        }

        private void ShortcutManagerOnActionsChanged(object sender, EventArgs eventArgs)
        {
            ReloadShortcutsMapping();
        }

        private void ReloadShortcutsMapping()
        {
            if (mDeserializedShortcuts == null)
                return;

            Shortcuts.Clear();
            foreach (var shortcut in mDeserializedShortcuts)
            {
                var callback =
                    Callbacks.FirstOrDefault(
                        c => string.Compare(c.Name, shortcut.ShortcutAction.Name, StringComparison.OrdinalIgnoreCase) == 0);
                if (callback != null)
                {
                    shortcut.ShortcutAction = callback;
                    Shortcuts.Add(shortcut);
                    mShortcutManager.MapAction(shortcut.ShortcutText, callback);
                }
            }
            OnPropertyChanged("");
        }

        private void Serialize()
        {
            var file = new FileStream("Shotcuts.txt", FileMode.Create);
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(mShortcuts);
            var writer = new StreamWriter(file) { AutoFlush = true };
            writer.WriteLine(json);
            file.Close();
        }

        private void Deserialize()
        {
            Stream file = null;
            try
            {
                file = new FileStream("Shotcuts.txt", FileMode.Open);
                var reader = new StreamReader(file);
                var json = reader.ReadToEnd();
                var deserializer = new JavaScriptSerializer();
                mDeserializedShortcuts = (ObservableCollection<Shortcut>)deserializer.Deserialize(json, typeof(ObservableCollection<Shortcut>));              
                file.Close();
            }
            catch (Exception)
            {
                if (file != null)
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
                    mAddNewShortcut = new DelegateCommand(param => AddShortcut());
                }
                return mAddNewShortcut;
            }
        }

        private DelegateCommand mUpdateShortcut;
        public ICommand UpdateShortcut
        {
            get
            {
                if (mUpdateShortcut == null)
                {
                    mUpdateShortcut = new DelegateCommand(param => UpdateShortcutFunc(param));
                }
                return mUpdateShortcut;
            }
        }

        private void UpdateShortcutFunc(object o)
        {
            var text = o as string;
            var shortcut = text == null? null : 
                Shortcuts.FirstOrDefault(s => string.Compare(s.ShortcutText, text, StringComparison.OrdinalIgnoreCase) == 0);
            if (shortcut != null)
            {
                mShortcutManager.UnMapAction(shortcut.ShortcutText);
                mShortcutManager.MapAction(shortcut.ShortcutText, shortcut.ShortcutAction);
            }
        }

        private void AddShortcut()
        {
            if (NewShortcutText != null && ComboSelectedItem != null && Callbacks != null && mShortcutManager.MapAction(NewShortcutText, ComboSelectedItem))
            {
                mShortcuts.Add(new Shortcut(NewShortcutText, ComboSelectedItem));
            }
            OnPropertyChanged("Shortcuts");
        }

        private DelegateCommand mSaveShortcuts;
        public ICommand SaveShortcuts
        {
            get
            {
                if (mSaveShortcuts == null)
                {
                    mSaveShortcuts = new DelegateCommand(param =>
                        {
                            Serialize();
                            Deserialize();
                        });
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
                    mDeleteShortcut = new DelegateCommand(param => DeleteShC());
                }
                return mDeleteShortcut;
            }
        }

        private void DeleteShC()
        {
            var listSelectedItem = ListSelectedItem;
            mShortcuts.Remove(listSelectedItem);
            mShortcutManager.UnMapAction(listSelectedItem.ShortcutText);
        }
    }
}
