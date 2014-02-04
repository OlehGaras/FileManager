using System.Windows.Controls;

namespace FileManager
{
    public interface IViewController
    {
        void SetLeftPanelContent(UserControl content);
        void SetRightPanelContent(UserControl content);
    }
}
