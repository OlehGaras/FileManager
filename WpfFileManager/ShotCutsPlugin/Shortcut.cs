using System;
using FileManager;

namespace ShotCutsPlugin
{
    [Serializable]
    public class Shortcut
    {
        public string ShortcutText { get; set; }

        private ShortcutAction mAction;
        public ShortcutAction ShortcutAction {
            get { return mAction; }
            set { mAction = value; } 
        }

        public Shortcut()
        {
        }

        public Shortcut(string text)
        {
            ShortcutText = text;
        }

        public Shortcut(string text, ShortcutAction currCallback)
            : this(text)
        {
            ShortcutAction = currCallback;
        }
    }
}