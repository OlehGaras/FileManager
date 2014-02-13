using System;
using System.ComponentModel.Composition;
using FileManager;

namespace testPlugin
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        private readonly IViewController mViewController;
        private readonly ICurrentFileSystemState mCurrentFileSystemState;
        private readonly IShortcutManager mShortcutManager;
        private readonly IErrorManager mErrorManager;

        [ImportingConstructor]
        public Plugin(IViewController viewController, ICurrentFileSystemState currentFileSystemState, IShortcutManager shortcutManager,IErrorManager errorManager)
        {
            if (viewController == null)
                throw new ArgumentNullException("viewController");
            if (currentFileSystemState == null)
                throw new ArgumentNullException("currentFileSystemState");
            if (shortcutManager == null)
                throw new ArgumentNullException("shortcutManager");
            if(errorManager == null)
                throw new ArgumentNullException("errorManager");
            mViewController = viewController;
            mCurrentFileSystemState = currentFileSystemState;
            mShortcutManager = shortcutManager;
            mErrorManager = errorManager;
        }

        public Guid PluginGuid { get; private set; }
        private Guid PluginViewGuid { get; set; }

        public Version PluginVersion { get { return new Version(1, 0); } }
        public Version AppVersion { get { return new Version(1, 0); } }
        public void Apply(Guid pluginGuid)
        {
            PluginGuid = pluginGuid;

            var leftDirectoryViewModel = new DirectoryViewModel(mCurrentFileSystemState ,mErrorManager, Panel.Left);
            var leftPanel = new DirectoryView(leftDirectoryViewModel);
            var rightDirectoryViewModel = new DirectoryViewModel(mCurrentFileSystemState,mErrorManager, Panel.Right);
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
            mViewController.CloseToolPanel(PluginGuid, PluginViewGuid);
        }
    }
}
