using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using FileManager;

namespace ShotCutsPlugin
{
    [Serializable]
    public class Shortcut
    {
        public string ShortcutText { get; set; }
        [ScriptIgnore]
        public List<ShortcutAction> Functions { get; set; }

        public ShortcutAction CurrCallback { get; set; }

        public Shortcut()
        {
        }

        public Shortcut(string text, List<ShortcutAction> registeredCallbacks)
        {
            ShortcutText = text;
            Functions = registeredCallbacks;
        }

        public Shortcut(string text, ShortcutAction currCallback, List<ShortcutAction> registeredCallbacks)
            : this(text, registeredCallbacks)
        {
            CurrCallback = currCallback;
        }
    }
}