using System;
using System.Runtime.InteropServices;

namespace Legacy.Core.ServiceWrapper
{
	public struct UPLAY_USER_CdKey
	{
		public Int32 unknown;

		[MarshalAs(UnmanagedType.LPStr)]
		public String keyUtf8;
	}
}
