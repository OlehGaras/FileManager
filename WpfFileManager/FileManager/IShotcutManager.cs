using System;
using System.Collections.Generic;

namespace FileManager
{
    public interface IShotcutManager
    {
        List<Callback> Functions { get; set; }
        void AddAction(Callback action);
        event EventHandler AvailableFunctionsChanged;
    }
}
