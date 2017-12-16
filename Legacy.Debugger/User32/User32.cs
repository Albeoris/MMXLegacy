using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Legacy.Debugger
{
    public static class User32
    {
        public delegate Boolean EnumWindowsPredicate(IntPtr windowHandle, IntPtr stateRef);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean EnumWindows(EnumWindowsPredicate check, IntPtr stateRef);

        [DllImport("user32", SetLastError = true)]
        public static extern Boolean EnumChildWindows(IntPtr windowHandle, EnumWindowsPredicate check, IntPtr stateRef);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Int32 GetWindowText(IntPtr windowHandle, StringBuilder sb, Int32 count);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr windowHandle, WindowMessage wm, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr windowHandle, WindowMessage wm, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 GetWindowThreadProcessId(IntPtr windowHandle, out Int32 processId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean SendNotifyMessage(IntPtr windowHandle, UInt32 messageId, UIntPtr wParam, String lParam);

        public static void Broadcast()
        {
            const Int32 HWND_BROADCAST = 0xffff;
            const UInt32 WM_SETTINGCHANGE = 0x001a;

            SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE, (UIntPtr)0, "Environment");
        }

        public static Boolean IsTitleEquals(IntPtr windowHandle, String title)
        {
            StringBuilder sb = new StringBuilder(title.Length + 2);
            Int32 size = GetWindowText(windowHandle, sb, sb.Capacity);
            if (size != title.Length)
                return false;

            return sb.ToString().StartsWith(title);
        }

        public static Boolean CloseWindow(IntPtr windowHandle)
        {
            return SendMessage(windowHandle, WindowMessage.Close, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero;
        }
    }
}