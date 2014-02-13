using System;
using System.Reflection;
using System.Windows;
using FileManager;

namespace WpfFileManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var pluginPath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "Plugins");
            var assembly1 = Assembly.LoadFrom(pluginPath + @"\testPlugin.dll");
            var assembly2 = Assembly.LoadFrom(pluginPath + @"\ShotCutsPlugin.dll");
            var assembly3 = Assembly.LoadFrom(pluginPath + @"\ErrorPlugin.dll");
            var assembly4 = Assembly.LoadFile(pluginPath + @"\MoveCopyPlugin.dll");

            ServiceLocator.Current.RegisterAssemblyTypes(typeof(App).Assembly);
            ServiceLocator.Current.RegisterAssemblyTypes(assembly1);
            ServiceLocator.Current.RegisterAssemblyTypes(assembly2);
            ServiceLocator.Current.RegisterAssemblyTypes(assembly3);
            ServiceLocator.Current.RegisterAssemblyTypes(assembly4);

            var mainWindowViewModel = ServiceLocator.Current.GetInstance<MainWindowViewModel>();
            ServiceLocator.Current.Register(new MainWindow(mainWindowViewModel));

            var mainWindow = ServiceLocator.Current.GetInstance<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();
            mainWindowViewModel.ApplyPlugins();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            var mainWindowViewModel = ServiceLocator.Current.GetInstance<MainWindowViewModel>();
            mainWindowViewModel.Dispose();
        }
    }
}
