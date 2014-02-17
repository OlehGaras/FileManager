using System;
using System.ComponentModel.Composition;
using FileManager;

namespace MoveCopyPlugin
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        private readonly IShortcutManager mShortcutManager;
        private IErrorManager mErrorManager;
        private readonly IViewController mViewController;
        private readonly ICurrentFileSystemState mCurrentFileSystemState;
        private Guid mPluginViewGuid;

        [ImportingConstructor]
        public Plugin(IShortcutManager shortcutManager, IErrorManager errorManager, IViewController viewController, ICurrentFileSystemState currentFileSystemState)
        {
            if (shortcutManager == null) 
                throw new ArgumentNullException("shortcutManager");
            if (errorManager == null) 
                throw new ArgumentNullException("errorManager");
            if (viewController == null) 
                throw new ArgumentNullException("viewController");
            if (currentFileSystemState == null) 
                throw new ArgumentNullException("currentFileSystemState");
            mShortcutManager = shortcutManager;
            mErrorManager = errorManager;
            mViewController = viewController;
            mCurrentFileSystemState = currentFileSystemState;
        }

        public Guid PluginGuid { get; private set; }
       
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

        public void Apply(Guid pluginGuid)
        {
            PluginGuid = pluginGuid;
            var moveCopyViewModel = new MoveCopyViewModel(mCurrentFileSystemState, mErrorManager);
            var moveCopyView = new MoveCopyView(moveCopyViewModel);

            mPluginViewGuid = mViewController.AddToolPanel(pluginGuid, moveCopyView, "Moving/Coping Process");

            mShortcutManager.AddAction(new ShortcutAction("Copy", () =>
                {
                    moveCopyViewModel.BackgroundWorker.DoWork += moveCopyViewModel.Copy;             
                }));
            mShortcutManager.AddAction(new ShortcutAction("Paste", () => { }));
            mShortcutManager.AddAction(new ShortcutAction("Move", moveCopyViewModel.Move));
            mShortcutManager.AddAction(new ShortcutAction("Delete", moveCopyViewModel.Delete));
        }

        public void Dispose()
        {
            mViewController.CloseToolPanel(PluginGuid,mPluginViewGuid);
        }
    }
}
