using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.ServiceWrapper
{
	public class DefaultServiceWrapperStrategy : IServiceWrapperStrategy
	{
		public event EventHandler<ActionCompletedEventArgs> OnActionCompleted;

		public void Init()
		{
		}

		public void Close()
		{
		}

		public void EarnAchievment(Int32 achievementId)
		{
		}

		public Boolean IsConnected()
		{
			return false;
		}

		public Boolean IsOfflineMode()
		{
			return false;
		}

		public Boolean IsConnectedToServer()
		{
			return false;
		}

		public Boolean ForceAppExit()
		{
			return false;
		}

		public Boolean UplayNotInstalled()
		{
			return false;
		}

		public void Update()
		{
		}

		public void ShowOverlay()
		{
		}

		public String GetUserName()
		{
			return String.Empty;
		}

		public String GetPassword()
		{
			return String.Empty;
		}

		public String GetActionName(Int32 p_actionId)
		{
			return String.Empty;
		}

		public void GetCDKeyList()
		{
		}

		public void UpdatePrivilegesRewards()
		{
		}

		public Boolean IsPrivilegeAvailable(Int32 m_id)
		{
			return m_id == 1001 || m_id == 1002 || m_id == 1003;
		}

		public Boolean IsRewardAvailable(Int32 m_id)
		{
			return false;
		}

		public void UnlockPrivilege(String p_key, Int32 p_expectedPrivilegeId)
		{
		}

		public void UnlockPrivilege(String p_key)
		{
		}

		public EActivateKeyResult GetUnlockPrivilegeState()
		{
			return EActivateKeyResult.ACTIVATE_UNEXPECTED_ERROR;
		}

		public void CancelUnlockPrivilege()
		{
		}

		public void SendTrackingData(String tagName, String attributes)
		{
		}

		public void SendTrackingDataAndWait(String tagName, String attributes)
		{
		}

		public void SetSaveGameManager()
		{
			LegacyLogic.Instance.WorldManager.SaveGameManager = new DefaultSaveGameManager();
		}
	}
}
