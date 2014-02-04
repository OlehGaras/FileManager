using System;
using System.ComponentModel.Composition;
using FileManager;

namespace testPlugin
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        private readonly IViewController mViewController;
        private readonly ICurrentDirectory mCurrentDirectory;

        [ImportingConstructor]
        public Plugin(IViewController viewController, ICurrentDirectory currentDirectory)
        {
            if (viewController == null) 
                throw new ArgumentNullException("viewController");
            if (currentDirectory == null) 
                throw new ArgumentNullException("currentDirectory");
            mViewController = viewController;
            mCurrentDirectory = currentDirectory;
        }

        public Version PluginVersion { get{ return new Version(1, 0);} }
        public Version AppVersion { get { return new Version(1, 0); } }
        public void Apply()
        {
            var leftPanel = new DirectoryView(new DirectoryViewModel(mCurrentDirectory, Panel.Left));
            var rightPanel = new DirectoryView(new DirectoryViewModel(mCurrentDirectory, Panel.Right));
            mViewController.SetLeftPanelContent(leftPanel);
            mViewController.SetRightPanelContent(rightPanel);
        }
    }
}
