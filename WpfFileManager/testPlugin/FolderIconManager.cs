using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace testPlugin
{
    public static class FolderManager
    {
        public static ImageSource GetImageSource(string directory, ShellManager.ItemState folderType)
        {
            return GetImageSource(directory, new Size(32, 32), folderType);
        }

        public static ImageSource GetImageSource(string directory, Size size, ShellManager.ItemState folderType)
        {
            using (var icon = ShellManager.GetIcon(directory, ShellManager.ItemType.Folder, ShellManager.IconSize.Large, folderType))
            {
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight((int)size.Width, (int)size.Height));
            }
        }
    }


}
