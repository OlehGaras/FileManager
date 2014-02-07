﻿using System;
using System.Web.Script.Serialization;

namespace FileManager
{
    [Serializable]
    public class ShortcutAction
    {
        public string Name { get; set; }

        [ScriptIgnore]
        public Callback Action { get; set; }

        public ShortcutAction(string name, Callback action)
        {
            Name = name;
            Action = action;
        }
    }
}
