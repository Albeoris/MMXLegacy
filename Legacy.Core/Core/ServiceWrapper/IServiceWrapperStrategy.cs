using System;

namespace Legacy.Core.ServiceWrapper
{
	public interface IServiceWrapperStrategy
	{
		event EventHandler<ActionCompletedEventArgs> OnActionCompleted;

		void Init();

		void Close();

		void EarnAchievment(Int32 p_achievementId);

		Boolean IsConnected();

		Boolean IsOfflineMode();

		Boolean IsConnectedToServer();

		Boolean ForceAppExit();

		Boolean UplayNotInstalled();

		void Update();

		void ShowOverlay();

		String GetUserName();

		String GetPassword();

		String GetActionName(Int32 p_actionId);

		void GetCDKeyList();

		void UpdatePrivilegesRewards();

		void UnlockPrivilege(String p_key, Int32 p_expectedPrivilegeId);

		void UnlockPrivilege(String p_key);

		EActivateKeyResult GetUnlockPrivilegeState();

		void CancelUnlockPrivilege();

		Boolean IsPrivilegeAvailable(Int32 p_id);

		Boolean IsRewardAvailable(Int32 p_id);

		void SendTrackingData(String tagName, String attributes);

		void SendTrackingDataAndWait(String tagName, String attributes);

		void SetSaveGameManager();
	}
}
