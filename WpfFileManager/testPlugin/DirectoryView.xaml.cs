using System.Windows.Controls;

namespace testPlugin
{
    /// <summary>
    /// Interaction logic for DirectoryView.xaml
    /// </summary>
    public partial class DirectoryView : UserControl
    {
        public DirectoryView(DirectoryViewModel directoryViewModel)
        {
            DataContext = directoryViewModel;
            InitializeComponent();
        }
    }
}
