using System;
using System.Runtime.InteropServices;

namespace Legacy.Core.Utilities
{
	public static class SystemInvokes
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern MessageBoxResult MessageBox(IntPtr hWnd, String text, String caption, Int32 options);
	}
}
