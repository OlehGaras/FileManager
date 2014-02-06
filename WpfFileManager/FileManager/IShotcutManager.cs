using System;
using System.Collections.Generic;

namespace FileManager
{
    public interface IShotcutManager
    {
        //List<ShortcutAction> Functions { get; }
        List<ShortcutAction> GetActions(); 
        void AddAction(ShortcutAction action);
        event EventHandler AvailableFunctionsChanged;
    }
}
