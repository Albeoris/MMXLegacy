using System;
using System.Runtime.InteropServices;

namespace Legacy.Core.ServiceWrapper
{
	public static class RendezVousInvokes
	{
		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousStartUp(String sandboxId, String accessKey, String localeCode, String gameCode, String platformCode, Boolean enableLog);

		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousStartUpTrackingOnly(String sandboxId, String accessKey, Boolean enableLog);

		[DllImport("LegacyRendezVous.dll")]
		public static extern Int32 LegacyRendezVousGetStartUpState();

		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousLogIn(String userName, String password, String onlineKey);

		[DllImport("LegacyRendezVous.dll")]
		public static extern Int32 LegacyRendezVousGetLogInState();

		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousShutDown();

		[DllImport("LegacyRendezVous.dll")]
		public static extern IntPtr LegacyRendezVousGetPrivileges(out Int32 count);

		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousActivateKey(String key, Int32 expectedPrivilege);

		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousActivateAnyKey(String key);

		[DllImport("LegacyRendezVous.dll")]
		public static extern Int32 LegacyRendezVousGetActivateKeyState();

		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousCancelActivateKey();

		[DllImport("LegacyRendezVous.dll")]
		public static extern IntPtr LegacyRendezVousGetRewards(out Int32 count);

		[DllImport("LegacyRendezVous.dll")]
		public static extern IntPtr LegacyRendezVousGetActionName(Int32 actionId);

		[DllImport("LegacyRendezVous.dll")]
		public static extern Boolean LegacyRendezVousSetActionsCompleted(Boolean action1, Boolean action2, Boolean action3, Boolean action4);

		[DllImport("LegacyRendezVous.dll")]
		public static extern Int32 LegacyRendezVousGetActionsState();

		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousSendTag(String tagName, String attributes);

		[DllImport("LegacyRendezVous.dll")]
		public static extern void LegacyRendezVousSendTagAndWait(String tagName, String attributes);

		[DllImport("LegacyRendezVous.dll")]
		public static extern IntPtr LegacyRendezVousGetLocation();
	}
}
