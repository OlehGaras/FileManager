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
        private readonly IShortcutManager mShortcutManager;

        [ImportingConstructor]
        public Plugin(IViewController viewController, ICurrentDirectory currentDirectory, IShortcutManager shortcutManager)
        {
            if (viewController == null)
                throw new ArgumentNullException("viewController");
            if (currentDirectory == null)
                throw new ArgumentNullException("currentDirectory");
            if (shortcutManager == null)
                throw new ArgumentNullException("shortcutManager");
            mViewController = viewController;
            mCurrentDirectory = currentDirectory;
            mShortcutManager = shortcutManager;

        }

        public Guid PluginGuid { get; private set; }
        public Guid PluginViewGuid { get; private set; }

        public Version PluginVersion { get { return new Version(1, 0); } }
        public Version AppVersion { get { return new Version(1, 0); } }
        public void Apply(Guid pluginGuid)
        {
            PluginGuid = pluginGuid;

            var leftDirectoryViewModel = new DirectoryViewModel(mCurrentDirectory, mViewController, Panel.Left);
            var leftPanel = new DirectoryView(leftDirectoryViewModel);
            var rightDirectoryViewModel = new DirectoryViewModel(mCurrentDirectory, mViewController, Panel.Right);
            var rightPanel = new DirectoryView(rightDirectoryViewModel);
            mViewController.SetLeftPanelContent(leftPanel);
            mViewController.SetRightPanelContent(rightPanel);
            mShortcutManager.AddAction(new ShortcutAction("HelloWorld Action", () => Console.WriteLine("HelloWorld")));
            mShortcutManager.AddAction(new ShortcutAction("ByeWorld Action", () => Console.WriteLine("ByeWorld")));
            mShortcutManager.AddAction(new ShortcutAction("Change Style", () =>
            {
                leftDirectoryViewModel.ChangeStyle();
                rightDirectoryViewModel.ChangeStyle();
            }
            ));
        }

        public void Dispose()
        {
            mViewController.CloseToolPanel(PluginViewGuid);
        }
    }
}
