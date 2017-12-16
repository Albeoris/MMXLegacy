using System;
using System.Runtime.InteropServices;

namespace Legacy.Core.ServiceWrapper
{
	public struct UPLAY_SAVE_Game
	{
		public UInt32 id;

		[MarshalAs(UnmanagedType.LPStr)]
		public String nameUtf8;

		public UInt32 size;
	}
}
