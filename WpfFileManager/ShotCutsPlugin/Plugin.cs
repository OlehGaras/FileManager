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
        private readonly IShortcutManager mAvailableFunctions;
        private Guid mGuid;

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
        public Plugin(IViewController viewController,IShortcutManager availableFunctions)
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
            mGuid = mViewController.AddToolPanel(shortcutPanel, "Shortcuts");
        }

        public void Dispose()
        {
            mViewController.CloseToolPanel(mGuid);
        }
    }
}
