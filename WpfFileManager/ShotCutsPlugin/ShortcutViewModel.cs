using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using FileManager;

namespace ShotCutsPlugin
{
    public class ShortcutViewModel : ViewModelBase
    {
        private readonly IShotcutManager mAvailableFunctions;
        //private readonly Shortcuts mShortcuts;
        private List<Callback> mCallbacks = new List<Callback>();
        public List<Callback> Callbacks
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

        public ShortcutViewModel(IShotcutManager availableFunctions)
        {
            mAvailableFunctions = availableFunctions;
            mAvailableFunctions.AvailableFunctionsChanged += (sender, args) => mCallbacks.AddRange(mAvailableFunctions.Functions);
            mShortcuts = new List<Shortcut>() { new Shortcut("ctrl+c", mCallbacks), new Shortcut("ctrl+v", mCallbacks) };
        }

        private List<Shortcut> mShortcuts;// = new List<Shortcut>() { new Shortcut("ctrl+c",new List<Callback>()), new Shortcut("ctrl+v", new List<Callback>()) };
        public List<Shortcut> Shortcuts
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
            // The text box grabs all input.
            k.Handled = true;

            // Fetch the actual shortcut key.
            var key = (k.Key == Key.System ? k.SystemKey : k.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            // Build the shortcut key name.
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

            // Update the text box.
            NewShortcutText = shortcutText.ToString();
        }
    }
}
