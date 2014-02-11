using System;
using System.Collections.Generic;

namespace FileManager
{
    public interface IShortcutManager
    {
        List<ShortcutAction> GetActions(); 
        void AddAction(ShortcutAction action);
        ShortcutAction GetActionByKey(string key);
        bool MapAction(string key, ShortcutAction action);
        void UnMapAction(string key);
        event EventHandler ActionsChanged;
    }
}
