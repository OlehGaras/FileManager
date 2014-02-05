using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using FileManager;

namespace ShotCutsPlugin
{
    [Export(typeof(IPlugin))]
    public class Plugin:IPlugin
    {
        private readonly IViewController mViewController;
        private readonly IShotcutManager mAvailableFunctions;

        public Version PluginVersion {
            get
            {
                return new Version(1,0);
            }
        }
        public Version AppVersion {
            get
            {
                return new Version(1,0);
            }
        }


        [ImportingConstructor]
        public Plugin(IViewController viewController,IShotcutManager availableFunctions)
        {
            if (viewController != null) 
                mViewController = viewController;
            if (availableFunctions != null)
                mAvailableFunctions = availableFunctions;
        }

        public List<Callback> RegisterAvailableFunctions()
        {
            return new List<Callback>();
        }

        public void Apply()
        {
            var shortcuts = new Shortcuts();
            var shortcutPanel = new ShortcutView(new ShortcutViewModel(mAvailableFunctions));
            mViewController.SetShortcutPanelContent(shortcutPanel);
        }
    }
}
