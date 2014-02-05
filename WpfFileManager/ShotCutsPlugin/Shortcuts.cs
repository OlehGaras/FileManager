using System.Collections.Generic;
using FileManager;

namespace ShotCutsPlugin
{
    public class Shortcut
    {
        public string ShortcutText { get; set; }
        public List<Callback> Functions { get; set; }
        public Callback CurrCallback { get; set; }

        public Shortcut(string text, List<Callback> registeredCallbacks)
        {
            ShortcutText = text;
            Functions = registeredCallbacks;
        }
    }

    public class Shortcuts
    {
        public List<Shortcut> ShortcutsWithCallbacks = new List<Shortcut>();

        //public Shortcuts(List<Shortcut> shortcuts)
        //{
        //    ShortcutsWithCallbacks = shortcuts;
        //}
        
        public void Add(Shortcut shortcut)
        {
            ShortcutsWithCallbacks.Add(shortcut);
        }
    }
}
