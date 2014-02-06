using System;
using System.Collections.Generic;

namespace FileManager
{
    public interface IShotcutManager
    {
        List<ShortcutAction> GetActions(); 
        void AddAction(ShortcutAction action);
        event EventHandler AvailableFunctionsChanged;
        event EventHandler<string> ShortcutPressed;
    }
}
