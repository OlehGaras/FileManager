using System;

namespace FileManager
{
    public delegate void Callback();
    public interface IPlugin
    {
        Version PluginVersion { get; }
        Version AppVersion { get; }
        void Apply();
    }
}
