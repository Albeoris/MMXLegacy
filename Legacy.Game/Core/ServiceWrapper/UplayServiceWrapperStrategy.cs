using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Utilities;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Core.ServiceWrapper
{
	public class UplayServiceWrapperStrategy : IServiceWrapperStrategy
	{
		private const String SANDBOX_ID = "a95554daed1b42848ecd43690d2ca976";

		private const String SANDBOX_ACCESS_KEY = "3zLCAurt";

		private const UInt32 UPLAY_APP_ID = 403u;

		private const UInt32 UPLAY_APP_VERSION = 9u;

		private Boolean m_isUplayConnected;

		private Boolean m_startingRendezVous;

		private Boolean m_connectingToRendezVous;

		private Int32 m_unlockingKeyNr;

		private Boolean m_isRendezVousConnected;

		private Boolean m_isOfflineMode;

		private Boolean m_isForceQuit;

		private Boolean m_uplayNotInstalled;

		private List<String> m_keys = new List<String>();

		private Int32[] m_privileges;

		private Int32[] m_rewards;

		private Boolean m_isSettingAction;

		private Int32 m_settingActionID = -1;

		private Dictionary<IntPtr, UPLAY_Overlapped> overlappedMap = new Dictionary<IntPtr, UPLAY_Overlapped>();

		public UplayServiceWrapperStrategy()
		{
			m_isUplayConnected = false;
			m_isForceQuit = false;
			Init();
		}

		public event EventHandler<ActionCompletedEventArgs> OnActionCompleted;

		public void Init()
		{
			String languageCode = GetLanguageCode();
			try
			{
				switch (UplayInvokes.UPLAY_Startup(403u, 9u, languageCode))
				{
				case 0:
					m_isUplayConnected = true;
					break;
				case 1:
				{
					String empty = String.Empty;
					UplayInvokes.UPLAY_GetLastError(ref empty);
					Debug.Log("Uplay Error: " + empty);
					break;
				}
				case 2:
					m_isForceQuit = true;
					break;
				case 3:
					Screen.SetResolution(640, 480, false);
					m_uplayNotInstalled = true;
					m_isForceQuit = true;
					break;
				}
			}
			catch (Exception exception)
			{
				m_isUplayConnected = false;
				m_isForceQuit = true;
				Debug.LogException(exception);
				Debug.Log("Exception connecting!");
			}
			if (!m_isForceQuit && m_isUplayConnected)
			{
				SetSaveGameManager();
				ConnectToServer();
				LoadPrivilegesRewards();
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SAVEGAME_LOADED, new EventHandler(OnSaveGameLoaded));
		}

		public void SetSaveGameManager()
		{
			if (m_isUplayConnected)
			{
				LegacyLogic.Instance.WorldManager.SaveGameManager = new UplaySaveGameManager();
			}
		}

		private void ConnectToServer()
		{
			String languageCode = GetLanguageCode();
			try
			{
				RendezVousInvokes.LegacyRendezVousStartUp("a95554daed1b42848ecd43690d2ca976", "3zLCAurt", languageCode, "MMX", "PC", ConfigManager.Instance.Game.EnableRendezVousLogging);
				m_startingRendezVous = true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				Debug.Log("Exception connecting!");
			}
		}

		private String GetLanguageCode()
		{
			String language = ConfigManager.Instance.Options.Language;
			String text = language;
			switch (text)
			{
			case "fr":
				return "fr-FR";
			case "de":
				return "de-DE";
			case "hu":
				return "hu-HU";
			case "cz":
				return "cs-CZ";
			case "it":
				return "it-IT";
			case "ro":
				return "ro-RO";
			case "pl":
				return "pl-PL";
			case "ru":
				return "ru-RU";
			case "cn":
				return "zh-CN";
			case "es":
				return "es-ES";
			case "br":
				return "pt-BR";
			case "jp":
				return "ja-JP";
			}
			return "en-US";
		}

		public void Close()
		{
			if (m_isUplayConnected)
			{
				RendezVousInvokes.LegacyRendezVousShutDown();
				UplayInvokes.UPLAY_Quit();
			}
			foreach (IntPtr hglobal in overlappedMap.Keys)
			{
				Marshal.FreeHGlobal(hglobal);
			}
			overlappedMap.Clear();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SAVEGAME_LOADED, new EventHandler(OnSaveGameLoaded));
		}

		public void EarnAchievment(Int32 achievementId)
		{
			if (m_isUplayConnected)
			{
				UPLAY_Overlapped uplay_Overlapped = default(UPLAY_Overlapped);
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uplay_Overlapped));
				Marshal.StructureToPtr(uplay_Overlapped, intPtr, true);
				overlappedMap.Add(intPtr, uplay_Overlapped);
				Int32 num = UplayInvokes.UPLAY_ACH_EarnAchievement((UInt32)achievementId, intPtr);
				if (num != 0)
				{
				}
			}
		}

		public Boolean IsConnected()
		{
			return m_isUplayConnected;
		}

		public Boolean IsOfflineMode()
		{
			return m_isOfflineMode;
		}

		public Boolean IsConnectedToServer()
		{
			return m_isRendezVousConnected;
		}

		public Boolean ForceAppExit()
		{
			return m_isForceQuit;
		}

		public Boolean UplayNotInstalled()
		{
			return m_uplayNotInstalled;
		}

		public void Update()
		{
			if (m_startingRendezVous)
			{
				StartUpState startUpState = (StartUpState)RendezVousInvokes.LegacyRendezVousGetStartUpState();
				if (startUpState == StartUpState.SUCCESSFUL)
				{
					m_startingRendezVous = false;
					String userName = GetUserName();
					String password = GetPassword();
					GetCDKeyList();
					if (m_keys.Count > 0)
					{
						RendezVousInvokes.LegacyRendezVousLogIn(userName, password, m_keys[0]);
						m_connectingToRendezVous = true;
						m_unlockingKeyNr = -1;
					}
				}
				else if (startUpState == StartUpState.FAILURE)
				{
					m_startingRendezVous = false;
					m_isOfflineMode = true;
				}
			}
			if (m_connectingToRendezVous)
			{
				if (m_unlockingKeyNr == -1)
				{
					LogInState logInState = (LogInState)RendezVousInvokes.LegacyRendezVousGetLogInState();
					if (logInState == LogInState.SUCCESSFUL)
					{
						m_unlockingKeyNr = 0;
						RendezVousInvokes.LegacyRendezVousActivateAnyKey(m_keys[m_unlockingKeyNr]);
					}
					else if (logInState == LogInState.FAILURE)
					{
						m_isOfflineMode = true;
						m_connectingToRendezVous = false;
					}
				}
				else
				{
					EActivateKeyResult unlockPrivilegeState = GetUnlockPrivilegeState();
					if (unlockPrivilegeState != EActivateKeyResult.ACTIVATE_WAITING)
					{
						m_unlockingKeyNr++;
						if (m_unlockingKeyNr < m_keys.Count)
						{
							RendezVousInvokes.LegacyRendezVousActivateAnyKey(m_keys[m_unlockingKeyNr]);
						}
						else
						{
							DoneLogin();
						}
					}
				}
			}
			if (m_isSettingAction)
			{
				SetActionState setActionState = (SetActionState)RendezVousInvokes.LegacyRendezVousGetActionsState();
				if (setActionState == SetActionState.SUCCESSFUL)
				{
					if (OnActionCompleted != null && m_settingActionID >= 0)
					{
						ActionCompletedEventArgs e = new ActionCompletedEventArgs(m_settingActionID, true);
						OnActionCompleted(this, e);
					}
					m_isSettingAction = false;
					m_settingActionID = -1;
				}
				else if (setActionState == SetActionState.FAILURE)
				{
					if (OnActionCompleted != null && m_settingActionID >= 0)
					{
						ActionCompletedEventArgs e2 = new ActionCompletedEventArgs(m_settingActionID, false);
						OnActionCompleted(this, e2);
					}
					m_isSettingAction = false;
					m_settingActionID = -1;
				}
			}
			List<IntPtr> list = new List<IntPtr>();
			foreach (IntPtr intPtr in overlappedMap.Keys)
			{
				if (UplayInvokes.UPLAY_HasOverlappedOperationCompleted(intPtr))
				{
					list.Add(intPtr);
				}
			}
			foreach (IntPtr intPtr2 in list)
			{
				overlappedMap.Remove(intPtr2);
				Marshal.FreeHGlobal(intPtr2);
			}
			list.Clear();
			if (UplayInvokes.UPLAY_Update() == 0)
			{
				m_isForceQuit = true;
			}
			UPLAY_Event uplay_Event;
			while (UplayInvokes.UPLAY_GetNextEvent(out uplay_Event))
			{
				Debug.Log("Received uplay Event! : " + uplay_Event.type);
				if (uplay_Event.type == UPLAY_EventType.UPLAY_Event_UserAccountSharing)
				{
					m_isRendezVousConnected = false;
					m_isOfflineMode = true;
				}
			}
		}

		private void DoneLogin()
		{
			m_isOfflineMode = (UplayInvokes.UPLAY_USER_IsInOfflineMode() != 0);
			m_isRendezVousConnected = !m_isOfflineMode;
			m_connectingToRendezVous = false;
			UpdatePrivilegesRewards();
			IntPtr ptr = RendezVousInvokes.LegacyRendezVousGetLocation();
			LegacyLogic.Instance.TrackingManager.TrackGameStart(Marshal.PtrToStringAnsi(ptr));
		}

		public void ShowOverlay()
		{
			UPLAY_Overlapped uplay_Overlapped = default(UPLAY_Overlapped);
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uplay_Overlapped));
			Marshal.StructureToPtr(uplay_Overlapped, intPtr, true);
			overlappedMap.Add(intPtr, uplay_Overlapped);
			UplayInvokes.UPLAY_OVERLAY_Show(UPLAY_OVERLAY_Section.UPLAY_OverlaySection_Show, intPtr);
		}

		public String GetUserName()
		{
			if (m_isUplayConnected)
			{
				IntPtr ptr = UplayInvokes.UPLAY_USER_GetUsernameUtf8();
				return Marshal.PtrToStringAnsi(ptr);
			}
			return String.Empty;
		}

		public String GetPassword()
		{
			if (m_isUplayConnected)
			{
				IntPtr ptr = UplayInvokes.UPLAY_USER_GetPasswordUtf8();
				return Marshal.PtrToStringAnsi(ptr);
			}
			return String.Empty;
		}

		public String GetActionName(Int32 p_actionId)
		{
			if (m_isRendezVousConnected)
			{
				IntPtr ptr = RendezVousInvokes.LegacyRendezVousGetActionName(p_actionId);
				return Marshal.PtrToStringAnsi(ptr);
			}
			return String.Empty;
		}

		public void GetCDKeyList()
		{
			UPLAY_Overlapped uplay_Overlapped = default(UPLAY_Overlapped);
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uplay_Overlapped));
			Marshal.StructureToPtr(uplay_Overlapped, intPtr, true);
			overlappedMap.Add(intPtr, uplay_Overlapped);
			IntPtr ptr;
			Int32 num = UplayInvokes.UPLAY_USER_GetCdKeys(out ptr, intPtr);
			if (num != 0)
			{
				UplayInvokes.UPLAY_Update();
				while (!UplayInvokes.UPLAY_HasOverlappedOperationCompleted(intPtr))
				{
					Thread.Sleep(5);
					UplayInvokes.UPLAY_Update();
				}
				m_keys.Clear();
				UPLAY_USER_CdKeyList uplay_USER_CdKeyList = (UPLAY_USER_CdKeyList)Marshal.PtrToStructure(ptr, typeof(UPLAY_USER_CdKeyList));
				IntPtr list = uplay_USER_CdKeyList.list;
				for (Int32 i = 0; i < uplay_USER_CdKeyList.count; i++)
				{
					UPLAY_USER_CdKey uplay_USER_CdKey = (UPLAY_USER_CdKey)Marshal.PtrToStructure(list, typeof(UPLAY_USER_CdKey));
					m_keys.Add(uplay_USER_CdKey.keyUtf8);
					list = new IntPtr(list.ToInt32() + IntPtr.Size);
				}
			}
		}

		public void OnTokenAdded(Object p_sender, EventArgs p_args)
		{
			ETokenID tokenID = (ETokenID)((TokenEventArgs)p_args).TokenID;
			if ((tokenID == ETokenID.TOKEN_STORY_ACT_1 || tokenID == ETokenID.TOKEN_STORY_ACT_2 || tokenID == ETokenID.TOKEN_STORY_ACT_3 || tokenID == ETokenID.TOKEN_STORY_ACT_4) && !m_isSettingAction)
			{
				m_settingActionID = tokenID - ETokenID.TOKEN_STORY_ACT_1 + 1;
				UpdateActions();
			}
		}

		public void OnSaveGameLoaded(Object p_sender, EventArgs p_args)
		{
			UpdateActions();
		}

		public void UpdateActions()
		{
			if (m_isRendezVousConnected)
			{
				TokenHandler tokenHandler = LegacyLogic.Instance.WorldManager.Party.TokenHandler;
				Boolean flag = tokenHandler.GetTokens(7) > 0;
				Boolean flag2 = tokenHandler.GetTokens(8) > 0;
				Boolean flag3 = tokenHandler.GetTokens(9) > 0;
				Boolean flag4 = tokenHandler.GetTokens(10) > 0;
				if (flag || flag2 || flag3 || flag4)
				{
					RendezVousInvokes.LegacyRendezVousSetActionsCompleted(flag, flag2, flag3, flag4);
					m_isSettingAction = true;
				}
			}
		}

		public void UpdatePrivilegesRewards()
		{
			if (m_isRendezVousConnected)
			{
				Int32 num;
				IntPtr source = RendezVousInvokes.LegacyRendezVousGetPrivileges(out num);
				m_privileges = new Int32[num];
				if (num > 0)
				{
					Marshal.Copy(source, m_privileges, 0, num);
				}
				Int32 num2;
				IntPtr source2 = RendezVousInvokes.LegacyRendezVousGetRewards(out num2);
				m_rewards = new Int32[num2];
				if (num2 > 0)
				{
					Marshal.Copy(source2, m_rewards, 0, num2);
				}
				SavePrivilegesRewards();
			}
		}

		public void SavePrivilegesRewards()
		{
			SaveGameData saveGameData = new SaveGameData("PrivilegesRewards");
			saveGameData.Set<Int32>("Count", m_privileges.Length);
			for (Int32 i = 0; i < m_privileges.Length; i++)
			{
				saveGameData.Set<Int32>("id" + i, m_privileges[i]);
			}
			saveGameData.Set<Int32>("RewardsCount", m_rewards.Length);
			for (Int32 j = 0; j < m_rewards.Length; j++)
			{
				saveGameData.Set<Int32>("rewardId" + j, m_rewards[j]);
			}
			LegacyLogic.Instance.WorldManager.SaveGameManager.SaveSaveGameData(saveGameData, "global2.lsg");
		}

		public void LoadPrivilegesRewards()
		{
			SaveGameData saveGameData = new SaveGameData("PrivilegesRewards");
			LegacyLogic.Instance.WorldManager.SaveGameManager.LoadSaveGameData(saveGameData, "global2.lsg");
			m_privileges = new Int32[saveGameData.Get<Int32>("Count", 0)];
			for (Int32 i = 0; i < m_privileges.Length; i++)
			{
				m_privileges[i] = saveGameData.Get<Int32>("id" + i, 0);
			}
			m_rewards = new Int32[saveGameData.Get<Int32>("RewardsCount", 0)];
			for (Int32 j = 0; j < m_rewards.Length; j++)
			{
				m_rewards[j] = saveGameData.Get<Int32>("rewardId" + j, 0);
			}
		}

		public Boolean IsPrivilegeAvailable(Int32 p_id)
		{
			for (Int32 i = 0; i < m_privileges.Length; i++)
			{
				if (m_privileges[i] == p_id)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean IsRewardAvailable(Int32 p_id)
		{
			for (Int32 i = 0; i < m_rewards.Length; i++)
			{
				if (m_rewards[i] == p_id)
				{
					return true;
				}
			}
			return false;
		}

		public void UnlockPrivilege(String p_key, Int32 p_expectedPrivilegeId)
		{
			if (m_isRendezVousConnected)
			{
				RendezVousInvokes.LegacyRendezVousActivateKey(p_key, p_expectedPrivilegeId);
			}
		}

		public void UnlockPrivilege(String p_key)
		{
			if (m_isRendezVousConnected)
			{
				LegacyLogger.LogError("try to unlock key");
				RendezVousInvokes.LegacyRendezVousActivateAnyKey(p_key);
			}
		}

		public EActivateKeyResult GetUnlockPrivilegeState()
		{
			if (m_isRendezVousConnected)
			{
				return (EActivateKeyResult)RendezVousInvokes.LegacyRendezVousGetActivateKeyState();
			}
			return EActivateKeyResult.ACTIVATE_UNEXPECTED_ERROR;
		}

		public void CancelUnlockPrivilege()
		{
			RendezVousInvokes.LegacyRendezVousCancelActivateKey();
		}

		public void SendTrackingData(String tagName, String attributes)
		{
			if (m_isUplayConnected)
			{
				RendezVousInvokes.LegacyRendezVousSendTag(tagName, attributes);
			}
		}

		public void SendTrackingDataAndWait(String tagName, String attributes)
		{
			if (m_isUplayConnected)
			{
				RendezVousInvokes.LegacyRendezVousSendTag(tagName, attributes);
				Thread.Sleep(500);
			}
		}

		private enum LogInState
		{
			LOGIN_AUTHENTICATING = -4,
			LOGIN_FETCHING_PRIVILEGES,
			LOGIN_FETCHING_ACTIONS,
			LOGIN_FETCHING_LOCATION,
			FAILURE,
			SUCCESSFUL
		}

		private enum StartUpState
		{
			WAITING = -1,
			FAILURE,
			SUCCESSFUL
		}

		private enum SetActionState
		{
			WAITING = -1,
			FAILURE,
			SUCCESSFUL
		}
	}
}
