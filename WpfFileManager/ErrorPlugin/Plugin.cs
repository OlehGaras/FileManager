using System;
using System.ComponentModel.Composition;
using FileManager;

namespace ErrorPlugin
{
    [Export(typeof(IPlugin))]
    public class Plugin:IPlugin
    {
        private readonly IViewController mViewController;
        private readonly IErrorManager mErrorManager;

        public Guid PluginGuid { get; private set; }
        public Guid PluginViewGuid { get; private set; }

        public Version PluginVersion
        {
            get
            {
                return new Version(1, 0);
            }
        }
        public Version AppVersion
        {
            get
            {
                return new Version(1, 0);
            }
        }

        [ImportingConstructor]
        public Plugin(IViewController viewController, IErrorManager errorManager)
        {
            if (viewController != null) 
                mViewController = viewController;
            if (errorManager != null) 
                mErrorManager = errorManager;
        }

        public void Apply(Guid pluginGuid)
        {
            PluginGuid = pluginGuid;
            var panel = new ErrorView(new ErrorViewModel(mErrorManager));
            PluginViewGuid = mViewController.AddToolPanel(pluginGuid, panel, "Errors");

        }
        public void Dispose()
        {
            mViewController.CloseToolPanel(PluginGuid,PluginViewGuid);
        }
    }
}
