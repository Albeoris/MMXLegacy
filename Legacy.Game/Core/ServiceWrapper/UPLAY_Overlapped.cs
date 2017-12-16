using System;
using System.Runtime.InteropServices;

namespace Legacy.Core.ServiceWrapper
{
	public struct UPLAY_Overlapped
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public UInt64[] _internal;
	}
}
