using System;
using System.Windows;

namespace WpfFileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            if (mainWindowViewModel == null)
                throw new ArgumentNullException("mainWindowViewModel");
            DataContext = mainWindowViewModel;
            InitializeComponent();
        }
    }
}
