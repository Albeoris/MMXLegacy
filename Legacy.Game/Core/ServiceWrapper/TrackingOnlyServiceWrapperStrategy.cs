using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.ServiceWrapper
{
	public class TrackingOnlyServiceWrapperStrategy : IServiceWrapperStrategy
	{
		private const String SANDBOX_ID = "a95554daed1b42848ecd43690d2ca976";

		private const String SANDBOX_ACCESS_KEY = "3zLCAurt";

		private Boolean m_startingRendezVous;

		public TrackingOnlyServiceWrapperStrategy()
		{
			Init();
		}

		public event EventHandler<ActionCompletedEventArgs> OnActionCompleted;

		public void Init()
		{
			RendezVousInvokes.LegacyRendezVousStartUpTrackingOnly("a95554daed1b42848ecd43690d2ca976", "3zLCAurt", ConfigManager.Instance.Game.EnableRendezVousLogging);
			m_startingRendezVous = true;
		}

		public void Close()
		{
			RendezVousInvokes.LegacyRendezVousShutDown();
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
			if (m_startingRendezVous)
			{
				StartUpState startUpState = (StartUpState)RendezVousInvokes.LegacyRendezVousGetStartUpState();
				if (startUpState == StartUpState.SUCCESSFUL)
				{
					m_startingRendezVous = false;
					LegacyLogic.Instance.TrackingManager.TrackGameStart("China");
				}
				else if (startUpState == StartUpState.FAILURE)
				{
					m_startingRendezVous = false;
				}
			}
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
			return m_id == 1001;
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
			RendezVousInvokes.LegacyRendezVousSendTag(tagName, attributes);
		}

		public void SendTrackingDataAndWait(String tagName, String attributes)
		{
			RendezVousInvokes.LegacyRendezVousSendTag(tagName, attributes);
			Thread.Sleep(500);
		}

		public void SetSaveGameManager()
		{
			LegacyLogic.Instance.WorldManager.SaveGameManager = new DefaultSaveGameManager();
		}

		private enum StartUpState
		{
			WAITING = -1,
			FAILURE,
			SUCCESSFUL
		}
	}
}
