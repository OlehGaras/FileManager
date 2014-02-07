using System.Windows;
using System.Windows.Controls;

namespace testPlugin
{
    public class PropertyDataTemplateSelector:DataTemplateSelector
    {
        public DataTemplate DirectoryInfoDataTemplate { get; set; }
        public DataTemplate FileInfoDataTemplate { get; set; }
        public DataTemplate DoublePointInfoDataTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var dpi = item as IFileSystemInfo;
            if (dpi != null && dpi.GetType() == typeof(DirectoryInfo))
            {
                return DirectoryInfoDataTemplate;
            }
            if (dpi != null && dpi.GetType() == typeof(FileInfo))
            {
                return FileInfoDataTemplate;
            }

            return DoublePointInfoDataTemplate;
        }
    }
}
