using System.Windows.Controls;

namespace ShotCutsPlugin
{
    /// <summary>
    /// Interaction logic for ShortcutView.xaml
    /// </summary>
    public partial class ShortcutView : UserControl
    {
        public ShortcutView(ShortcutViewModel shortcutViewModel)
        {
            DataContext = shortcutViewModel;
            InitializeComponent();
        }
    }
}
