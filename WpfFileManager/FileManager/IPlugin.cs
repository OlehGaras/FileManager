using System;
using System.Collections.Generic;

namespace FileManager
{
    public delegate void Callback();
    public interface IPlugin
    {
        Version PluginVersion { get; }
        Version AppVersion { get; }
        //List<Callback> RegisterAvailableFunctions();
        void Apply();
    }
}
