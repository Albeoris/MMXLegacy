using System;
using System.Runtime.InteropServices;

namespace Legacy.Core.ServiceWrapper
{
	public static class UplayInvokes
	{
		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_Startup(UInt32 aUplayId, UInt32 aGameVersion, String aLanguageCountryCodeUtf8);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_Quit();

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_ACH_EarnAchievement(UInt32 aAchievementId, IntPtr aOverlapped);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_SAVE_GetSavegames(out IntPtr UPLAY_SAVE_GameList, IntPtr aOverlapped);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_SAVE_Open(UInt32 aSlotId, UPLAY_SAVE_Mode aMode, out UInt32 aOutSaveHandle, IntPtr aOverlapped);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_SAVE_Close(UInt32 aSaveHandle);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_SAVE_Read(UInt32 aSaveHandle, UInt32 aNumOfBytesToRead, UInt32 aOffset, IntPtr aOutBuffer, out UInt32 aOutNumOfBytesRead, IntPtr aOverlapped);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_SAVE_Write(UInt32 aSaveHandle, UInt32 aNumOfBytesToWrite, IntPtr aBuffer, IntPtr aOverlapped);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_SAVE_SetName(UInt32 aSaveHandle, [MarshalAs(UnmanagedType.LPStr)] String aNameUtf8);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_SAVE_Remove(UInt32 aSlotId, IntPtr aOverlapped);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Boolean UPLAY_GetNextEvent(out UPLAY_Event aEvent);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_Update();

		[DllImport("uplay_r1_loader.dll")]
		public static extern Boolean UPLAY_HasOverlappedOperationCompleted(IntPtr aOverlapped);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Boolean UPLAY_GetOverlappedOperationResult(IntPtr aOverlapped, out UPLAY_OverlappedResult aOutResult);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_OVERLAY_Show(UPLAY_OVERLAY_Section aOerlaySection, IntPtr aOverlapped);

		[DllImport("uplay_r1_loader.dll")]
		public static extern IntPtr UPLAY_USER_GetUsernameUtf8();

		[DllImport("uplay_r1_loader.dll")]
		public static extern IntPtr UPLAY_USER_GetPasswordUtf8();

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_USER_IsInOfflineMode();

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_GetLastError(ref String aOutErrorString);

		[DllImport("uplay_r1_loader.dll")]
		public static extern Int32 UPLAY_USER_GetCdKeys(out IntPtr aOutCdKeyList, IntPtr aOverlapped);
	}
}
