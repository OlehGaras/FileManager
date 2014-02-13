using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using FileManager;
using System.Linq;

namespace WpfFileManager
{
    [Export(typeof(IShortcutManager))]
    public class ShortcutManager : IShortcutManager
    {
        private readonly List<ShortcutAction> mFunctions = new List<ShortcutAction>();
        private readonly Dictionary<string, ShortcutAction> mShortcuts = new Dictionary<string, ShortcutAction>(StringComparer.OrdinalIgnoreCase);

        public List<ShortcutAction> GetActions()
        {
            return new List<ShortcutAction>(mFunctions);
        }

        public void AddAction(ShortcutAction action)
        {
            mFunctions.Add(action);
            OnActionsChanged();
        }

        public ShortcutAction GetActionByKey(string key)
        {
            if (mShortcuts.ContainsKey(key))
            {
                return mShortcuts[key];
            }
            return null;
        }

        public bool MapAction(string key, ShortcutAction action)
        {
            if (mShortcuts.ContainsKey(key))
            {
                return false;
            }
            if (mFunctions.All(f => f != action))
            {
                return false;
            }
            mShortcuts.Add(key, action);
            return true;
        }

        public void UnMapAction(string key)
        {
            if (mShortcuts.ContainsKey(key))
            {
                mShortcuts.Remove(key);
            }
        }

        public event EventHandler ActionsChanged;

        protected virtual void OnActionsChanged()
        {
            var handler = ActionsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }

    
}