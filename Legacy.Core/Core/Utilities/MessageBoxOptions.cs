using System;

namespace Legacy.Core.Utilities
{
	[Flags]
	public enum MessageBoxOptions : uint
	{
		OkOnly = 0u,
		OkCancel = 1u,
		AbortRetryIgnore = 2u,
		YesNoCancel = 3u,
		YesNo = 4u,
		RetryCancel = 5u,
		CancelTryContinue = 6u,
		IconHand = 16u,
		IconQuestion = 32u,
		IconExclamation = 48u,
		IconAsterisk = 64u,
		UserIcon = 128u,
		IconWarning = 48u,
		IconError = 16u,
		IconInformation = 64u,
		IconStop = 16u,
		DefButton1 = 0u,
		DefButton2 = 256u,
		DefButton3 = 512u,
		DefButton4 = 768u,
		ApplicationModal = 0u,
		SystemModal = 4096u,
		TaskModal = 8192u,
		Help = 16384u,
		NoFocus = 32768u,
		SetForeground = 65536u,
		DefaultDesktopOnly = 131072u,
		Topmost = 262144u,
		Right = 524288u,
		RTLReading = 1048576u
	}
}
