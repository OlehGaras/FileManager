using System;
using System.Runtime.InteropServices;

namespace testPlugin
{
    public static class Interop
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out ShFileInfo psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        public const uint ShgfiIcon = 0x000000100;
        public const uint ShgfiUsefileattributes = 0x000000010;
        public const uint ShgfiOpenicon = 0x000000002;
        public const uint ShgfiSmallicon = 0x000000001;
        public const uint ShgfiLargeicon = 0x000000000;
        public const uint FileAttributeDirectory = 0x00000010;
        public const uint FileAttributeFile = 0x00000100;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShFileInfo
    {
        public IntPtr hIcon;

        public int iIcon;

        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };
}
