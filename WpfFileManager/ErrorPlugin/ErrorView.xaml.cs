using System.Windows.Controls;

namespace ErrorPlugin
{
    /// <summary>
    /// Interaction logic for ErrorView.xaml
    /// </summary>
    public partial class ErrorView : UserControl
    {
        public ErrorView(ErrorViewModel errorViewModel)
        {
            DataContext = errorViewModel;
            InitializeComponent();
        }
    }
}
