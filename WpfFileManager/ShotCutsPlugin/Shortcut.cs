using System.Collections.Generic;
using FileManager;

namespace ShotCutsPlugin
{
    public class Shortcut
    {
        public string ShortcutText { get; set; }
        public List<ShortcutAction> Functions { get; set; }
        public ShortcutAction CurrCallback { get; set; }

        public Shortcut(string text, List<ShortcutAction> registeredCallbacks)
        {
            ShortcutText = text;
            Functions = registeredCallbacks;
        }
    }
}