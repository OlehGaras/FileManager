using System;

namespace FileManager
{
    public interface IPlugin
    {
        Version PluginVersion { get; }
        Version AppVersion { get; }
        void Apply();
    }
}
