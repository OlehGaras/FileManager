using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace testPlugin
{
    public class ShellManager
    {
        public enum IconSize : short
        {
            Small,
            Large
        }

        public enum ItemState : short
        {
            Undefined,
            Open,
            Close
        }

        public enum ItemType
        {
            Folder,
            File
        }

        public static Icon GetIcon(string path, ItemType type, IconSize size, ItemState state)
        {
            var flags = Interop.ShgfiIcon | Interop.ShgfiUsefileattributes;
            var attribute = Equals(type, ItemType.Folder) ? Interop.FileAttributeDirectory : Interop.FileAttributeFile;
            if (Equals(type, ItemType.Folder) && Equals(state, ItemState.Open))
            {
                flags += Interop.ShgfiOpenicon;
            }
            if (Equals(size, IconSize.Small))
            {
                flags += Interop.ShgfiSmallicon;
            }
            else
            {
                flags += Interop.ShgfiLargeicon;
            }
            var shfi = new ShFileInfo();
            var res = Interop.SHGetFileInfo(path, attribute, out shfi, (uint)Marshal.SizeOf(shfi), flags);
            if (Equals(res, IntPtr.Zero)) throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            try
            {
                Icon.FromHandle(shfi.hIcon);
                return (Icon) Icon.FromHandle(shfi.hIcon).Clone();
            }
            finally
            {
                Interop.DestroyIcon(shfi.hIcon);
            }
        }
    }
}
