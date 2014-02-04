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
            var assembly = Assembly.LoadFrom(pluginPath + @"\testPlugin.dll");
            ServiceLocator.Current.RegisterAssemblyTypes(typeof(App).Assembly);
            ServiceLocator.Current.RegisterAssemblyTypes(assembly);
            var mainWindowViewModel = ServiceLocator.Current.GetInstance<MainWindowViewModel>();
            ServiceLocator.Current.Register(new MainWindow(mainWindowViewModel));

            var mainWindow = ServiceLocator.Current.GetInstance<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();
            mainWindowViewModel.ApplyPlugins();
        }
    }
}
