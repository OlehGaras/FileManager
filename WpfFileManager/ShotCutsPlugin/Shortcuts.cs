using System.Collections.Generic;

namespace ShotCutsPlugin
{
    public class Shortcuts
    {
        public List<Shortcut> ShortcutsWithCallbacks = new List<Shortcut>();

        public void Add(Shortcut shortcut)
        {
            ShortcutsWithCallbacks.Add(shortcut);
        }
    }
}
