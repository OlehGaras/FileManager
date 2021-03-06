﻿using System;

namespace FileManager
{
    
    public delegate void Callback();
    public interface IPlugin : IDisposable
    {
        Guid PluginGuid { get; }
        Version PluginVersion { get; }
        Version AppVersion { get; }
        void Apply(Guid pluginGuid);
    }
}
