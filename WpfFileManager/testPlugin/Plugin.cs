using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using FileManager;

namespace testPlugin
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        private readonly IViewController mViewController;
        private readonly ICurrentDirectory mCurrentDirectory;
        private readonly IShotcutManager mShotcutManager;

        [ImportingConstructor]
        public Plugin(IViewController viewController, ICurrentDirectory currentDirectory, IShotcutManager shotcutManager)
        {
            if (viewController == null)
                throw new ArgumentNullException("viewController");
            if (currentDirectory == null)
                throw new ArgumentNullException("currentDirectory");
            if (shotcutManager == null) 
                throw new ArgumentNullException("shotcutManager");
            mViewController = viewController;
            mCurrentDirectory = currentDirectory;
            mShotcutManager = shotcutManager;
        }

        public Version PluginVersion { get { return new Version(1, 0); } }
        public Version AppVersion { get { return new Version(1, 0); } }

        public void Apply()
        {
            var leftPanel = new DirectoryView(new DirectoryViewModel(mCurrentDirectory, Panel.Left));
            var rightPanel = new DirectoryView(new DirectoryViewModel(mCurrentDirectory, Panel.Right));
            mViewController.SetLeftPanelContent(leftPanel);
            mViewController.SetRightPanelContent(rightPanel);
            mShotcutManager.AddAction(new Callback(() =>
            {
                var a = 5;
            }));
        }
    }
}
